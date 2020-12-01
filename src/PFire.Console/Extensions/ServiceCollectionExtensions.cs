using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PFire.Core;
using PFire.WindowsService.Services;

namespace PFire.WindowsService.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAll(this IServiceCollection serviceCollection, IHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            return serviceCollection
                   .AddHostedService<PFireServerService>()
                   .AddSingleton(x => new PFireServer(hostEnvironment.ContentRootPath));
        }
    }
}