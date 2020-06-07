using MediatR;

namespace Wrapperizer.Abstraction
{
    public interface IDomainEventHandler<in TDe> 
        : INotificationHandler<TDe> where TDe : INotification
    {
    }
}
