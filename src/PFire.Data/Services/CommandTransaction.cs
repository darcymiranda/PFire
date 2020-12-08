using System;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PFire.Common.Extensions;

namespace PFire.Data.Services
{
    public interface ICommandTransaction : IDisposable
    {
        Task<ValidationResult> Commit();
        Task Rollback();
        ICommand<T> CreateEntity<T>() where T : Entity, new();
        ICommand<T> UpdateEntity<T>(params int[] id) where T : Entity, new();
        Task<ValidationResult> DeleteEntity<T>(params int[] id) where T : Entity, new();
    }

    internal class CommandTransaction : ICommandTransaction
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly IServiceProvider _serviceProvider;
        private IDbContextTransaction _transaction;

        public CommandTransaction(IServiceProvider serviceProvider, IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _serviceProvider = serviceProvider;
        }

        public async Task<ValidationResult> Commit()
        {
            try
            {
                await _databaseContext.SaveChanges();
                await _transaction.CommitAsync();

                return new ValidationResult();
            }
            catch (Exception ex)
            {
                return new ValidationResult().AddError(ex);
            }
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public ICommand<T> CreateEntity<T>() where T : Entity, new()
        {
            return GetCommand<T>().Create();
        }

        public ICommand<T> UpdateEntity<T>(params int[] id) where T : Entity, new()
        {
            return GetCommand<T>().Update(id);
        }

        public Task<ValidationResult> DeleteEntity<T>(params int[] id) where T : Entity, new()
        {
            return GetCommand<T>().Delete(id).SaveChanges();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }

        public async Task BeginTransaction()
        {
            _transaction = await _databaseContext.BeginTransaction();
        }

        private Command<T> GetCommand<T>() where T : Entity, new()
        {
            var command = _serviceProvider.GetRequiredService<Command<T>>();
            command.Initialize(_databaseContext);

            return command;
        }
    }
}
