using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PFire.Infrastructure.Database;

namespace PFire.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceCollection,
                                                                IHostEnvironment hostEnvironment,
                                                                IConfiguration configuration)
        {
            var databaseName = configuration["DatabaseSettings:Name"];
            var databasePath = Path.Combine(hostEnvironment.ContentRootPath, databaseName);

            return serviceCollection
                .AddSingleton<IPFireDatabase>(x => new PFireDatabase(databasePath));
        }
    }
}
