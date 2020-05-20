using System.Threading.Tasks;

namespace Wrapperizer.Core.Abstraction
{
    public interface IDomainEventManager
    {
        Task Dispatch(IDomainEvent @event);
        Task Dispatch(params IDomainEvent[] events);
    }
}
