using System.Diagnostics.Metrics;

namespace AVS.Otel.APIContagem.Metrics
{
    public class OtelMetrics
    {
        // Users Counters
        public Counter<int> UsersAuthenticateCounter { get; }

        // Customers Counters
        public Counter<int> CustomersGetAllCounter { get; }
        public Counter<int> CustomersGetAllWithPaginationCounter { get; }
        public Counter<int> CustomersGetCounter { get; }

        public string MetricName { get; }

        public OtelMetrics(string meterName = "Ecommerce_Metrics")
        {
            var meter = new Meter(meterName);
            MetricName = meterName;

            // Users meters
            UsersAuthenticateCounter = meter.CreateCounter<int>("ecommerce_users_authenticate_count", "User");

            // Customers meters
            CustomersGetAllCounter = meter.CreateCounter<int>("ecommerce_customers_getall_count", "Customer");
            CustomersGetAllWithPaginationCounter = meter.CreateCounter<int>("ecommerce_customers_getallwithpagination_count", "Customer");
            CustomersGetCounter = meter.CreateCounter<int>("ecommerce_customers_get_count", "Customer");
        }
    }
}
