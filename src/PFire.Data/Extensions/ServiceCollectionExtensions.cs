using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFire.Data.Services;

namespace PFire.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration["DatabaseSettings:Name"];

            return serviceCollection.AddSingleton<IPFireDatabase, PFireDatabase>()
                                    .AddScoped<IDatabaseContext, DatabaseContext>()
                                    .AddDbContext<DatabaseContext>(x => x.UseSqlite(connectionString));
        }
    }
}
