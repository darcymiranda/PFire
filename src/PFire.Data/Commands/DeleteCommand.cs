using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using PFire.Data.Services;

namespace PFire.Data.Commands
{
    internal class DeleteCommand<T> : Command<T> where T : Entity, new()
    {
        public DeleteCommand(IValidator<T> validator) : base(validator) {}

        protected override async Task<T> Get(IDatabaseContext databaseContext, int[] id, CancellationToken cancellationToken)
        {
            return await databaseContext.Set<T>().FindAsync(id, cancellationToken);
        }

        protected override Task<ValidationResult> ValidateEntity(T entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ValidationResult());
        }

        protected override void Save(IDatabaseContext databaseContext, T entity)
        {
            databaseContext.Set<T>().Remove(entity);
        }
    }
}
