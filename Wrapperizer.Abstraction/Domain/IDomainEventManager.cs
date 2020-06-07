using System.Threading.Tasks;

namespace Wrapperizer.Abstraction.Domain
{
    public interface IDomainEventManager
    {
        Task Publish(IDomainEvent @event);
        Task Publish(params IDomainEvent[] events);
    }
}
