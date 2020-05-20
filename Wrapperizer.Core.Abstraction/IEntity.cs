using System.Collections.Generic;

namespace Wrapperizer.Core.Abstraction
{
    public interface IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}