using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite; 
using PFire.Infrastructure.Models;
using PFire.Infrastructure.Services;

namespace PFire.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string ServerSettings = "ServerSettings";

        public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            ServerSettings serverSettings = new(); 
            configuration.GetSection(ServerSettings).Bind(serverSettings);

            var connectionString = configuration.GetConnectionString("PFire");
            SqliteConnection connection = new(connectionString);

            if (serverSettings.OpenSqlConnection) {
                connection.Open();  
            }

            serviceCollection.Configure<ServerSettings>(configuration.GetSection(ServerSettings));

            return serviceCollection.AddScoped<IDatabaseContext, DatabaseContext>()
                                    .AddScoped<IDatabaseMigrator, DatabaseContext>()
                                    .AddDbContext<DatabaseContext>(options => options.UseSqlite(connection));
        }
    }
}
