using System;
using MediatR;

namespace Wrapperizer.Abstraction
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTimeOffset OccuredOn { get; }
    }
}
