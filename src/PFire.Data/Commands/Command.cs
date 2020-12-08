using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using PFire.Common.Extensions;
using PFire.Data.Services;

namespace PFire.Data.Commands
{
    public interface ICommand<out T> where T : Entity, new()
    {
        ICommand<T> Run(Action<T> action);
        ICommand<T> Run(Func<T, Task> action);
        Task<ValidationResult> SaveChanges(CancellationToken cancellationToken = default);
    }

    internal abstract class Command<T> : ICommand<T> where T : Entity, new()
    {
        private readonly List<Func<T, Task>> _actions;
        private readonly IValidator<T> _validator;
        private IDatabaseContext _databaseContext;
        private int[] _id;

        protected Command(IValidator<T> validator)
        {
            _validator = validator;
            _actions = new List<Func<T, Task>>();
        }

        public ICommand<T> Run(Action<T> action)
        {
            if (action != null)
            {
                _actions.Add(x =>
                {
                    action(x);

                    return Task.CompletedTask;
                });
            }

            return this;
        }

        public ICommand<T> Run(Func<T, Task> action)
        {
            if (action != null)
            {
                _actions.Add(action);
            }

            return this;
        }

        public async Task<ValidationResult> SaveChanges(CancellationToken cancellationToken = default)
        {
            var (entity, result) = await GetEntity(cancellationToken);

            if (!result.IsValid)
            {
                return result;
            }

            result = await ApplyActions(entity, cancellationToken);

            if (!result.IsValid)
            {
                return result;
            }

            result = await ValidateEntity(entity, cancellationToken);

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!result.IsValid)
            {
                return result;
            }

            return SaveEntity(entity);
        }

        private async Task<(T entity, ValidationResult result)> GetEntity(CancellationToken cancellationToken)
        {
            try
            {
                var entity = await Get(_databaseContext, _id, cancellationToken);

                return (entity, new ValidationResult());
            }
            catch (Exception ex)
            {
                var result = new ValidationResult().AddError(ex);

                return (default, result);
            }
        }

        private async Task<ValidationResult> ApplyActions(T entity, CancellationToken cancellationToken)
        {
            foreach (var action in _actions)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await action.Invoke(entity);
                }
                catch (Exception ex)
                {
                    return new ValidationResult().AddError(ex);
                }
            }

            return new ValidationResult();
        }

        protected virtual async Task<ValidationResult> ValidateEntity(T entity, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(entity, cancellationToken);

                return new ValidationResult().Merge(validationResult);
            }
            catch (Exception ex)
            {
                return new ValidationResult().AddError(ex);
            }
        }

        private ValidationResult SaveEntity(T entity)
        {
            try
            {
                Save(_databaseContext, entity);

                return new ValidationResult();
            }
            catch (Exception ex)
            {
                return new ValidationResult().AddError(ex);
            }
        }

        protected abstract Task<T> Get(IDatabaseContext databaseContext, int[] id, CancellationToken cancellationToken);
        protected abstract void Save(IDatabaseContext databaseContext, T entity);

        public void Initialize(IDatabaseContext databaseContext, int[] id = null)
        {
            _databaseContext = databaseContext;
            _id = id;
        }
    }
}
