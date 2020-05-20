namespace Wrapperizer.Core.Abstraction
{
    public interface IRepository<T>
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
