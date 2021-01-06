using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Wrapperizer.Repository.EntityFrameworkCore.Extensions;
using Wrapperizer.Repository.EntityFrameworkCore.Abstraction;

namespace Wrapperizer.Repository.EntityFrameworkCore
{
    public abstract class DomainEventAwareDbContext : DbContext, ITransactionalUnitOfWork
    {
        private readonly IMediator _domainEventManager;
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
            IMediator domainEventManager,
            ILogger<DomainEventAwareDbContext> logger
        ) : this(options)
        {
            _domainEventManager = domainEventManager;
            _logger = logger;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // TODO: soft delete and audit
            // this.HandleSoftDelete();
            // this.HandleAuditProperties();
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        // private void HandleSoftDelete()
        // {
        //     var softDeleteEntities = this.ChangeTracker.Entries().Where(
        //         entry => entry.Entity is ICanBeSoftDeleted && entry.State == EntityState.Deleted);
        //
        //     foreach (var softDeleteEntity in softDeleteEntities)
        //     {
        //         this.Entry(softDeleteEntity.Entity).Property("SoftDeleted").CurrentValue = true;
        //         this.Entry(softDeleteEntity.Entity).State = EntityState.Modified;
        //     }
        // }
        //
        // private void HandleAuditProperties()
        // {
        //     var auditEntities = this.ChangeTracker.Entries().Where(entry=> entry.Entity is ICanBeAudited
        //     && ( entry.State == EntityState.Added || entry.State == EntityState.Modified));
        //
        //     foreach (var auditEntity in auditEntities)
        //     {
        //         if( auditEntity.State == EntityState.Added )
        //             this.Entry(auditEntity.Entity).Property("CreatedOn").CurrentValue = DateTimeOffset.UtcNow;
        //         else if( auditEntity.State == EntityState.Modified )
        //             this.Entry(auditEntity.Entity).Property("UpdatedOn").CurrentValue = DateTimeOffset.UtcNow;
        //     }
        // }

        public async Task<bool> CommitAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _domainEventManager
                    .DispatchDomainEventsAsync(this)
                    .ConfigureAwait(false);

                var result = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false) >= 0;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong commiting entities: {Message}",
                    ex.GetInnerMostExceptionMessage());
                throw;
            }
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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

            _currentTransaction = await this.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)
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
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
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
