using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AVS.Otel.APIContagem.Data;

public class ContagemContext : DbContext
{
    public DbSet<HistoricoContagem>? Historicos { get; set; }

    public ContagemContext(DbContextOptions<ContagemContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        
    }
}