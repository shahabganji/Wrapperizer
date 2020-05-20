using MediatR;

namespace Wrapperizer.Core.Abstraction
{
    public interface IDomainEventHandler<in TDe> 
        : INotificationHandler<TDe> where TDe : IDomainEvent
    {
    }
}
