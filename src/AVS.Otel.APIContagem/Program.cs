
using AVS.Otel.APIContagem.Data;
using AVS.Otel.APIContagem.Tracing;
using DbUp;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Grafana.Loki;
using Serilog;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using AVS.Otel.APIContagem.Data.DatabaseFlavor;
using static AVS.Otel.APIContagem.Data.DatabaseFlavor.ProviderConfiguration;
using Npgsql;
using AVS.Otel.APIContagem.Metrics;

namespace AVS.Otel.APIContagem
{
    public class Program
    {
        public static void Main(string[] args)
         {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            var builder = WebApplication.CreateBuilder(args);

            // Documentacao do OpenTelemetry:
            // https://opentelemetry.io/docs/instrumentation/net/getting-started/

            // Integracao do OpenTelemetry com Jaeger e tambem Grafana Tempo em .NET:
            // https://github.com/open-telemetry/opentelemetry-dotnet/tree/e330e57b04fa3e51fe5d63b52bfff891fb5b7961/docs/trace/getting-started-jaeger#collect-and-visualize-traces-using-jaeger

            builder.Services.ConfigureProviderForContext<ContagemContext>(DetectDatabase(builder.Configuration));
            
            builder.Services.AddScoped<ContagemRepository>();
            builder.Services.AddSingleton<OtelMetrics>();

            builder.Services.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.GrafanaLoki(
                    builder.Configuration["Loki:Uri"]!,
                    new List<LokiLabel>()
                    {
                        new()
                        {
                            Key = "service_name",
                            Value = OpenTelemetryExtensions.ServiceName
                        },
                        new()
                        {
                            Key = "using_database",
                            Value = "true"
                        }
                    })
                .Enrich.WithSpan(new SpanOptions() { IncludeOperationName = true, IncludeTags = true })
                .CreateLogger());
                      
            var meters = new OtelMetrics();

             builder.Services.AddOpenTelemetry()
            .WithTracing((traceBuilder) =>
            {
                traceBuilder
                    .AddSource(OpenTelemetryExtensions.ServiceName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
                                serviceVersion: OpenTelemetryExtensions.ServiceVersion))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()                 
                    .AddSqlClientInstrumentation()
                    .AddConnectorNet()
                    .AddNpgsql()
                    .AddSource("SQLiteDataSource")                    
                    .AddOtlpExporter()
                    .AddConsoleExporter();
            })
            .WithMetrics((metricBuilder) =>
            {
                metricBuilder
                    //.AddSource(OpenTelemetryExtensions.ServiceName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
                                serviceVersion: OpenTelemetryExtensions.ServiceVersion))
                    .AddMeter(meters.MetricName)                                
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter()
                    .AddConsoleExporter();
            });

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseSerilogRequestLogging();

            app.MapControllers();

            app.Run();
        }
    }
}
