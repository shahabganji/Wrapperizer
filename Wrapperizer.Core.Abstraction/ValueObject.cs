using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wrapperizer.Core.Abstraction
{
// This base class comes from Jimmy Bogard, but with support of inheritance
// http://grabbagoft.blogspot.com/2007/06/generic-value-object-equality.html

    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        public override bool Equals(object obj)
        {
            if (!(obj is T valueObject))
                return false;

            return Equals(valueObject);
        }

        public override int GetHashCode()
        {
            var fields = GetFields(this);

            const int startValue = 17;
            const int multiplier = 59;

            return fields
                .Select(field => field.GetValue(this))
                .Where(value => value != null)
                .Aggregate(
                    startValue,
                    (current, value) => current * multiplier + value.GetHashCode());
        }

        public virtual bool Equals(T other)
        {
            if (ReferenceEquals(other, null))
                return false;

//            // this might cause problems with libraries that make proxy classes over the real type
//            if (this.GetType() != other.GetType())
//                return false;

            if (ReferenceEquals(this, other))
                return true;

            var fields = GetFields(this);

            foreach (var field in fields)
            {
                var value1 = field.GetValue(other);
                var value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                    return false;
            }

            return true;
        }

        private static IEnumerable<FieldInfo> GetFields(object obj)
        {
            var t = obj.GetType();

            var fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                if (t == null) continue;
                fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

                t = t.BaseType;
            }

            return fields;
        }

        public static bool operator ==(ValueObject<T> x, ValueObject<T> y)
        {
            if (ReferenceEquals(x, null ) && ReferenceEquals(y, null)) return true; // both are null
            
            if (ReferenceEquals(x , null) || ReferenceEquals(y, null)) return false; // one is null & the other is not

            return ReferenceEquals(x, y) || x.Equals(y);
        }

        public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }
    }
}
