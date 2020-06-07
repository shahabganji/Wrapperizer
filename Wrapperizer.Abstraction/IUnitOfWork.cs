using System.Threading;
using System.Threading.Tasks;

namespace Wrapperizer.Abstraction
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken);
    }
}
