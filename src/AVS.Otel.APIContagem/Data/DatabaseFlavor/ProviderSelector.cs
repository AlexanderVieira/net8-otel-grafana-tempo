using Microsoft.EntityFrameworkCore;
using static AVS.Otel.APIContagem.Data.DatabaseFlavor.ProviderConfiguration;

namespace AVS.Otel.APIContagem.Data.DatabaseFlavor
{
    public static class ProviderSelector
    {
        public static IServiceCollection ConfigureProviderForContext<TContext>(
            this IServiceCollection services,
            (DatabaseType, string) options) where TContext : DbContext
        {
            var (database, connString) = options;
            Build(connString);
            return database switch
            {
                DatabaseType.SqlServer => services.PersistStore<TContext>(With.SqlServer),
                DatabaseType.MySql => services.PersistStore<TContext>(With.MySql),
                DatabaseType.Postgre => services.PersistStore<TContext>(With.Postgre),
                DatabaseType.Sqlite => services.PersistStore<TContext>(With.Sqlite),

                _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
            };
        }

        public static Action<DbContextOptionsBuilder> WithProviderAutoSelection((DatabaseType, string) options)
        {
            var (database, connString) = options;
            Build(connString);
            return database switch
            {
                DatabaseType.SqlServer => With.SqlServer,
                DatabaseType.MySql => With.MySql,
                DatabaseType.Postgre => With.Postgre,
                DatabaseType.Sqlite => With.Sqlite,

                _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
            };
        }
        
    }
}
