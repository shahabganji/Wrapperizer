using System.Collections.Generic;

namespace Wrapperizer.Domain.Abstraction
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
