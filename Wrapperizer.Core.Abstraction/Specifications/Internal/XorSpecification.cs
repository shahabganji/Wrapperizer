using System;
using System.Linq.Expressions;

namespace Wrapperizer.Core.Abstraction.Specifications.Internal
{
    internal class XorSpecification<T>  : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public XorSpecification(Specification<T> left , Specification<T> right)
        {
            _left = left;
            _right = right;
        }
        
        public override Expression<Func<T, bool>> ToExpression()
        {
            var notLeftAndRight = _left.Not().And(_right);
            var leftAndNotRight = _left.And(_right.Not());

            var xOr = notLeftAndRight.Or(leftAndNotRight);

            return xOr.ToExpression();
        }
    }
}
