using System.Linq;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFire.Data.Commands;
using PFire.Data.Services;
using PFire.Data.Validators;

namespace PFire.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:PFire"];

            return serviceCollection.AddSingleton<IPFireDatabase, PFireDatabase>()
                                    .AddScoped<IDatabaseContext, DatabaseContext>()
                                    .AddDbContext<DatabaseContext>(options => options.UseSqlite(connectionString))
                                    .AddTransient<IReader, Reader>()
                                    .AddSingleton<ICommandTransactionProvider, CommandTransactionProvider>()
                                    .AddTransient<CommandTransaction>()
                                    .AddTransient(typeof(CreateCommand<>))
                                    .AddTransient(typeof(UpdateCommand<>))
                                    .AddTransient(typeof(DeleteCommand<>))
                                    .RegisterValidators();
        }

        private static IServiceCollection RegisterValidators(this IServiceCollection serviceCollection)
        {
            var iValidatorType = typeof(IValidator<>);
            var dataAnnotationsValidatorType = typeof(DataAnnotationsValidator<>);
            var validatorsNamespace = dataAnnotationsValidatorType.Namespace;

            typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                                               .Where(type => type.Namespace == validatorsNamespace)
                                               .Select(type => new
                                               {
                                                   type.BaseType,
                                                   Implementation = type
                                               })
                                               .Where(type => type.BaseType.IsGenericType)
                                               .Where(type => type.BaseType.GetGenericTypeDefinition() == dataAnnotationsValidatorType)
                                               .Select(type =>
                                               {
                                                   var baseType = type.BaseType.FindInterfaces((x, y) =>
                                                       x.IsGenericType && x.GetGenericTypeDefinition() == iValidatorType,
                                                   null)
                                                   .Single();

                                                   return new
                                                   {
                                                       BaseType = baseType,
                                                       type.Implementation
                                                   };
                                               })
                                               .ToList()
                                               .ForEach(reg => serviceCollection.AddSingleton(reg.BaseType, reg.Implementation));

            return serviceCollection;
        }
    }
}
