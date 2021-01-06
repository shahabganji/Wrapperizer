using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Repository.EntityFrameworkCore.Abstraction
{
    
    public interface ITransactionalUnitOfWork : IUnitOfWork
    {
        bool TransactionInProgress { get; }
        IDbContextTransaction GetCurrentTransaction();
        DbConnection GetDbConnection();
        
        Task ExecuteTransactionAsync(Func<IDbContextTransaction, Task> operation, CancellationToken cancellationToken);

        IExecutionStrategy CreateExecutionStrategy();
        
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = new CancellationToken());
        void RollbackTransaction();
        Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
    }
    

}
