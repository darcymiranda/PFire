using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PFire.Data.Commands
{
    public interface ICommandTransactionProvider
    {
        Task<ICommandTransaction> StartTransaction();
    }

    internal class CommandTransactionProvider : ICommandTransactionProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandTransactionProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ICommandTransaction> StartTransaction()
        {
            var commandTransaction = _serviceProvider.GetRequiredService<CommandTransaction>();

            await commandTransaction.BeginTransaction();

            return commandTransaction;
        }
    }
}
