using System;
using System.Linq.Expressions;

namespace Wrapperizer.Domain.Specifications.Internal
{
    internal sealed class IdentitySpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> ToExpression()
            => _ => true;
    }
}
