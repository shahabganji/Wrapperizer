using System.Threading;
using System.Threading.Tasks;

namespace Wrapperizer.Domain.Abstraction
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken);
    }
}
