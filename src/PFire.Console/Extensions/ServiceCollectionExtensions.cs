using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PFire.Common.Extensions;
using PFire.Console.Services;
using PFire.Core.Extensions;
using PFire.Infrastructure.Extensions;

namespace PFire.Console.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAll(this IServiceCollection services, IHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            return services
                .AddHostedService<PFireServerService>()
                .RegisterCore()
                .RegisterInfrastructure(configuration)
                .RegisterCommon(configuration);
        }
    }
}
