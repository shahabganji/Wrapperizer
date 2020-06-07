using System;
using System.Linq.Expressions;
using Wrapperizer.Abstraction.Specifications.Internal;

namespace Wrapperizer.Abstraction.Specifications
{
    public abstract class Specification<T>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly Specification<T> All = new IdentitySpecification<T>();

        public abstract Expression<Func<T, bool>> ToExpression();
        
        public bool IsSatisfiedBy(T entity)
            => this.ToExpression().Compile().Invoke(entity);

        public Specification<T> And(Specification<T> specification)
        {
            if (this == All)
                return specification;

            if (specification == All)
                return this;

            return new AndSpecification<T>(this, specification);
        }

        public Specification<T> Or(Specification<T> specification)
            => (this == All || specification == All)
                ? All
                : new OrSpecification<T>(this, specification);

        public Specification<T> Not()
            => new NotSpecification<T>(this);

        public Specification<T> Xor(Specification<T> specification)
            => new XorSpecification<T>(this, specification);


        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
            => specification.ToExpression();

        public static Specification<T> operator !(Specification<T> spec)
            => spec.Not();

        public static Specification<T> operator &(Specification<T> left, Specification<T> right)
            => left.And(right);

        public static Specification<T> operator |(Specification<T> left, Specification<T> right)
            => left.Or(right);
    }
}
