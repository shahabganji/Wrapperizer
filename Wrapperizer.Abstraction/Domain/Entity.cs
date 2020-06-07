using System;
using System.Collections.Generic;

namespace Wrapperizer.Abstraction.Domain
{
    public abstract class Entity<T> : IEntity
        // this should be done on struct, ValueTypes, and not classes, ReferenceTypes
        // and violates YAGNI principle
        // : IEquatable<Entity<T>> where T : struct
    {
        public T Id { get; protected set; }

        private List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        protected Entity(T id)
        {
            if(Equals(id, default))
                throw  new ArgumentException("The ID value cannot be the type's default value." , nameof(id));

            this.Id = id;
        }

        // EF Core requires an empty constructor
        protected Entity()
        {
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

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
        

        public override bool Equals(object obj)
        {
            if (!(obj is Entity<T> other)) return false;
            
            return ReferenceEquals(this, other) || this.Id.Equals(other.Id);
            
            
//            // this might cause problems with libraries that make proxy classes over the real type
//            if (this.GetType() != other.GetType())
//                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(Entity<T> left, Entity<T> right)
        {
            if (ReferenceEquals(left, null ) && ReferenceEquals(right, null)) return true; // both are null
            
            if (ReferenceEquals(left , null) || ReferenceEquals(right, null)) return false; // one is null & the other is not

            return ReferenceEquals(left, right) || left.Equals(right);
        }

        public static bool operator !=(Entity<T> left, Entity<T> right)
        {
            return !(left == right);
        }
    }
}
