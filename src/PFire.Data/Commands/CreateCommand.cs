using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using PFire.Data.Services;

namespace PFire.Data.Commands
{
    internal class CreateCommand<T> : Command<T> where T : Entity, new()
    {
        public CreateCommand(IValidator<T> validator) : base(validator) {}

        protected override async Task<T> Get(IDatabaseContext databaseContext, int[] id, CancellationToken cancellationToken)
        {
            await Task.Yield();

            return new T();
        }

        protected override void Save(IDatabaseContext databaseContext, T entity)
        {
            databaseContext.Set<T>().Add(entity);
        }
    }
}
