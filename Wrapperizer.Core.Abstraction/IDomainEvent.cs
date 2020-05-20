using System;
using MediatR;

namespace Wrapperizer.Core.Abstraction
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTimeOffset OccuredOn { get; }
    }
}
