using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PFire.Console.Extensions;
using PFire.Infrastructure.Services;
using Serilog;

namespace PFire.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await MigrateDatabase(host);

            await host.RunAsync();
        }

        private static async Task MigrateDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();

            await scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>().Migrate();
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
