using MediatR;

namespace Wrapperizer.Abstraction.Domain
{
    public interface IDomainEventHandler<in TDe> 
        : INotificationHandler<TDe> where TDe : INotification
    {
    }
}
