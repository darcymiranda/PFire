using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PFire.WindowsService.Extensions;

namespace PFire.WindowsService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseWindowsService()
                       .ConfigureServices((hostBuilderContext, serviceCollection) =>
                           serviceCollection.RegisterAll(hostBuilderContext.HostingEnvironment, hostBuilderContext.Configuration));
        }
    }
}