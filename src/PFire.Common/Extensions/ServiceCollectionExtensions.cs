using Microsoft.Extensions.DependencyInjection;
using PFire.Common.Services;

namespace PFire.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCommon(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IDateTimeService, DateTimeService>();
        }

    }
}
