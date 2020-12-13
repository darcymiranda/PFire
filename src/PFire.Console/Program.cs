using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PFire.Console.Extensions;
using Serilog;

namespace PFire.Console
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
                       .UseSerilog((hostingContext, loggerConfiguration) =>
                       {
                           loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                       })
                       .UseWindowsService()
                       .UseSystemd()
                       .ConfigureServices((hostBuilderContext, services) =>
                       {
                           services.RegisterAll(hostBuilderContext.Configuration);
                       });
        }
    }
}
