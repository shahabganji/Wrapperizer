using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Wrapperizer.Core.Abstraction;

namespace Wrapperizer.Repositories.Abstraction
{
    public interface  ITransactionalUnitOfWork : IUnitOfWork
    {
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
        void RollbackTransaction();
        IExecutionStrategy CreateExecutionStrategy();
    }
}
