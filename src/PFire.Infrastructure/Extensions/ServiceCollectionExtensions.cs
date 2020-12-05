using Microsoft.Extensions.DependencyInjection;
using PFire.Infrastructure.Database;

namespace PFire.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IPFireDatabase, PFireDatabase>();
        }
    }
}
