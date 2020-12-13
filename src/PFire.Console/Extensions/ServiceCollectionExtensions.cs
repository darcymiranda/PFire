using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFire.Common.Extensions;
using PFire.Console.Services;
using PFire.Core.Extensions;
using PFire.Data.Extensions;

namespace PFire.Console.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAll(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                   .AddHostedService<PFireServerService>()
                   .RegisterCore()
                   .RegisterInfrastructure(configuration)
                   .RegisterCommon();
        }
    }
}
