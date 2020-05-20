using System.Threading;
using System.Threading.Tasks;

namespace Wrapperizer.Core.Abstraction
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync(CancellationToken cancellationToken);
    }
}
