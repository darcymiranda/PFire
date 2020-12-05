using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace PFire.Console.Extensions
{
    internal static class LoggingBuilderExtensions
    {
        public static void AddLogging(this ILoggingBuilder loggingBuilder, IHostEnvironment hostEnvironment)
        {
            var loggerConfiguration = loggingBuilder.GenerateLoggerConfiguration(hostEnvironment);

            var logger = loggerConfiguration.CreateLogger();

            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(logger, true);
        }

        private static LoggerConfiguration GenerateLoggerConfiguration(this ILoggingBuilder loggingBuilder, IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsEnvironment("Local"))
            {
                return new LoggerConfiguration().MinimumLevel.Debug()
                                                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                                .MinimumLevel.Override("System", LogEventLevel.Information)
                                                .WriteTo.Console();
            }

            var serviceProvider = loggingBuilder.Services.BuildServiceProvider();

            return new LoggerConfiguration().MinimumLevel.Information()
                                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                                            .Enrich.FromLogContext()
                                            .Enrich.WithProperty("Application", hostEnvironment.ApplicationName)
                                            .Enrich.WithProperty("Environment", hostEnvironment.EnvironmentName)
                                            .SetupRollingFile(serviceProvider);
        }
    }
}
