using System.Collections.Generic;

namespace Wrapperizer.Abstraction.Domain
{
    public interface IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
