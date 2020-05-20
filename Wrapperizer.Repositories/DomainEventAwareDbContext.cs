using System;
using System.Threading;
using System.Threading.Tasks;
using Fotokar.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Repositories.Abstraction;
using Wrapperizer.Repositories.Extensions;

namespace Wrapperizer.Repositories
{
    public abstract class DomainEventAwareDbContext : DbContext, ITransactionalUnitOfWork
    {
        private readonly IDomainEventManager _domainEventManager;
        private readonly ILogger<DomainEventAwareDbContext> _logger;
        
        private IDbContextTransaction _currentTransaction;

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

        private static readonly string InvalidOperationException =
            $"You should call the {nameof(CommitAsync)} method for persisting changes.";

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
            => throw new InvalidOperationException(InvalidOperationException);

        public override int SaveChanges()
            => throw new InvalidOperationException(InvalidOperationException);

        public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
            throw new InvalidOperationException(InvalidOperationException);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
            => throw new InvalidOperationException(InvalidOperationException);

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await this.Database.BeginTransactionAsync(cancellationToken)
                .ConfigureAwait(false);

            return _currentTransaction;
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

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return this.Database.CreateExecutionStrategy();
        }
    }
}
