using System.Threading;
using System.Threading.Tasks;

namespace Wrapperizer.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken);
    }
}
