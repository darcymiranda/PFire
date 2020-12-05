using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFire.Common.Models;
using PFire.Common.Services;

namespace PFire.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCommon(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            return serviceCollection
                   .AddSettings(configuration)
                   .AddSingleton<IDateTimeService, DateTimeService>();
        }

        private static IServiceCollection AddSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            return serviceCollection
                   .ConfigureSettings<DatabaseSettings>(configuration)
                   .ConfigureSettings<LoggingSettings>(configuration);
        }

        private static IServiceCollection ConfigureSettings<T>(this IServiceCollection serviceCollection, IConfiguration configuration) where T : class
        {
            return serviceCollection.Configure<T>(configuration.GetSection(typeof(T).Name));
        }
    }
}
