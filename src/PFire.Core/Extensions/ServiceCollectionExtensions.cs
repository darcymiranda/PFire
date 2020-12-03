using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PFire.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCore(this IServiceCollection serviceCollection, IHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            return serviceCollection
                .AddSingleton<IPFireServer>(x => new PFireServer(hostEnvironment.ContentRootPath));
        }
    }
}