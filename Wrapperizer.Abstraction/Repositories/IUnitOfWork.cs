using System.Threading;
using System.Threading.Tasks;

namespace Wrapperizer.Abstraction.Repositories
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken);
    }
}
