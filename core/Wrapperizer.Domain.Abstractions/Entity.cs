using System;
using Wrapperizer.Domain.Abstractions.Extensions;

namespace Wrapperizer.Domain.Abstractions
{
    public abstract class Entity<T>
        // this should be done on struct, ValueTypes, and not classes, ReferenceTypes
        // and violates YAGNI principle
        // : IEquatable<Entity<T>> where T : struct
    {
        public T Id { get; }

       
        protected Entity(T id)
        {
            this.Id = id;
        }

        // EF Core requires an empty constructor
        protected Entity()
        {
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Entity<T> other)) return false;

            if (ReferenceEquals(this, other))
                return true;

            // this might cause problems with libraries that make proxy
            // classes over the real type, like lazy loading scenario
            // use the type.BaseType probably
            if (this.GetRealType() != other.GetRealType())
                return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return (this.GetRealType().ToString() + Id).GetHashCode();
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

    public abstract class Entity : Entity<Guid>
    {
        protected Entity(Guid guid)
        {
            if(Equals(guid, default))
                throw  new ArgumentException("The ID value cannot be the type's default value." , nameof(guid));

        }
    }
}
