namespace Wrapperizer.Domain.Abstractions
{
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
