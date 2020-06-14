using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;
using Wrapperizer.Extensions.Repositories.EfCore.Extensions;

namespace Wrapperizer.Extensions.Repositories.EfCore
{
    public abstract class DomainEventAwareDbContext : DbContext, ITransactionalUnitOfWork
    {
        private readonly IDomainEventManager _domainEventManager;
        private readonly ILogger<DomainEventAwareDbContext> _logger;

        private IDbContextTransaction _currentTransaction;
        
        public bool TransactionInProgress => _currentTransaction != null;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public DbConnection GetDbConnection() => this.Database.GetDbConnection();

        public DomainEventAwareDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DomainEventAwareDbContext(
            DbContextOptions options,
            IDomainEventManager domainEventManager,
            ILogger<DomainEventAwareDbContext> logger
        ) : this(options)
        {
            _domainEventManager = domainEventManager;
            _logger = logger;
        }
        
        public async Task<bool> CommitAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _domainEventManager
                    .DispatchDomainEventsAsync(this)
                    .ConfigureAwait(false);

                await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wen wrong commiting entities: {Message}",
                    ex.GetInnerMostExceptionMessage());
                return false;
            }
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return this.Database.CreateExecutionStrategy();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await this.Database.BeginTransactionAsync(cancellationToken)
                .ConfigureAwait(false);

            return _currentTransaction;
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task ExecuteTransactionAsync(Func<IDbContextTransaction, Task> operation,
            CancellationToken cancellationToken)
        {
            var strategy = this.Database.CreateExecutionStrategy();

            // ReSharper disable once HeapView.CanAvoidClosure
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await this.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                await operation(transaction).ConfigureAwait(false);

                await this.CommitTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}
