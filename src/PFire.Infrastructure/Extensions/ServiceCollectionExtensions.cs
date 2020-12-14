﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFire.Infrastructure.Services;

namespace PFire.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PFire");

            return serviceCollection.AddScoped<IDatabaseContext, DatabaseContext>()
                                    .AddScoped<IDatabaseMigrator, DatabaseContext>()
                                    .AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString));
        }
    }
}
