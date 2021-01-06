using System;
using System.Collections.Generic;

namespace Wrapperizer.Domain.Abstractions
{
    public abstract class AggregateRoot<T> : Entity<T> , IAggregateRoot
    {
        private List<IDomainEvent> _domainEvents;

        protected AggregateRoot(List<IDomainEvent> domainEvents)
        {
            _domainEvents = domainEvents;
        }

        protected AggregateRoot()
        {
            _domainEvents = new List<IDomainEvent>();
        }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        protected void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }

        protected void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }
    }

    public abstract class AggregateRoot : AggregateRoot<Guid>
    {
    }
}
