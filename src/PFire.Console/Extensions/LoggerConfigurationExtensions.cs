using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PFire.Common.Extensions;
using PFire.Common.Models;
using Serilog;

namespace PFire.Console.Extensions
{
    internal static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration SetupRollingFile(this LoggerConfiguration loggerConfiguration, IServiceProvider serviceProvider)
        {
            var fileSettings = serviceProvider.GetRequiredService<IOptions<LoggingSettings>>().Value.File;
            var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

            var path = Path.Combine(hostEnvironment.ContentRootPath, fileSettings.Path);

            return loggerConfiguration.WriteTo.File(path, rollingInterval: fileSettings.Interval.ToEnum<RollingInterval>());
        }
    }
}
