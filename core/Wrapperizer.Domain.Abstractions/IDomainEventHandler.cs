using MediatR;

namespace Wrapperizer.Domain.Abstractions
{
    public interface IDomainEventHandler<in TDe> 
        : INotificationHandler<TDe> where TDe : INotification
    {
    }
}
