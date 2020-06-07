using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Wrapperizer.Abstraction;

namespace Wrapperizer.Extensions.Repositories.EfCore.Abstraction
{
    
    public interface ITransactionalUnitOfWork : IUnitOfWork
    {
        bool TransactionInProgress { get; }
        
        Task ExecuteTransactionAsync(Func<IDbContextTransaction, Task> operation, CancellationToken cancellationToken);

        IExecutionStrategy CreateExecutionStrategy();
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = new CancellationToken());
        void RollbackTransaction();
        Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
    }
    

}
