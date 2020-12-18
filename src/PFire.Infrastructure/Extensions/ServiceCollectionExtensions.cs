using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFire.Infrastructure.Services;
using Microsoft.Data.Sqlite; 

namespace PFire.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const bool openConnectionNeeded = false;

        public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PFire");
            SqliteConnection connection = new(connectionString);

            if (openConnectionNeeded) {
                #pragma warning disable CS0162
                connection.Open();  
                #pragma warning restore CS0162
            }

            return serviceCollection.AddScoped<IDatabaseContext, DatabaseContext>()
                                    .AddScoped<IDatabaseMigrator, DatabaseContext>()
                                    .AddDbContext<DatabaseContext>(options => options.UseSqlite(connection));
        }
    }
}
