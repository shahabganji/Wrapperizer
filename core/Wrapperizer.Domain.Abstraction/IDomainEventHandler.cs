using MediatR;

namespace Wrapperizer.Domain.Abstraction
{
    public interface IDomainEventHandler<in TDe> 
        : INotificationHandler<TDe> where TDe : INotification
    {
    }
}
