using System.Collections.Generic;

namespace Wrapperizer.Abstraction
{
    public interface IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
