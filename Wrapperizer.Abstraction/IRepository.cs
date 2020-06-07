namespace Wrapperizer.Abstraction
{
    public interface IRepository<T>
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
