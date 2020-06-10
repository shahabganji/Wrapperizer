using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Abstraction.Repositories
{
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
