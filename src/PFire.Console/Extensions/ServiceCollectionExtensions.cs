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
        public static IServiceCollection RegisterAll(this IServiceCollection serviceCollection, IHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            return serviceCollection.AddHostedService<PFireServerService>()
                                    .RegisterCore()
                                    .RegisterInfrastructure(hostEnvironment, configuration)
                                    .RegisterCommon(configuration)
                                    .AddLogging(builder => builder.AddLogging(hostEnvironment));
        }
    }
}
