using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using PFire.Data.Services;

namespace PFire.Data.Commands
{
    internal class UpdateCommand<T> : Command<T> where T : Entity, new()
    {
        public UpdateCommand(IValidator<T> validator) : base(validator) {}

        protected override async Task<T> Get(IDatabaseContext databaseContext, int[] id, CancellationToken cancellationToken)
        {
            return await databaseContext.Set<T>().FindAsync(id, cancellationToken);
        }

        protected override void Save(IDatabaseContext databaseContext, T entity)
        {
            databaseContext.Set<T>().Update(entity);
        }
    }
}
