using System;
using MediatR;

namespace Wrapperizer.Abstraction.Domain
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTimeOffset OccuredOn { get; }
    }
}
