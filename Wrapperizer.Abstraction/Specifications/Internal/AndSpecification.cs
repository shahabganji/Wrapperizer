using System;
using System.Linq.Expressions;

namespace Wrapperizer.Abstraction.Specifications.Internal
{
    internal class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpression = _left.ToExpression();
            var rightExpression = _right.ToExpression();

            var invokedExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    leftExpression.Body, invokedExpression),
                leftExpression.Parameters
            );
        }
    }
}
