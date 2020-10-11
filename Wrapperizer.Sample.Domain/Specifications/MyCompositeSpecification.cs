using System;
using System.Linq.Expressions;
using Wrapperizer.Abstraction.Specifications;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Domain.Specifications
{
    public sealed class MyCompositeSpecification : Specification<Student>
    {
        public override Expression<Func<Student, bool>> ToExpression()
        {
            var doubleZeroSpec = new NationalCodeStartWithDoubleZeroSpecification();
            var pendingSpec = new PendingStudentSpecification().And(doubleZeroSpec);

            return pendingSpec.ToExpression();
        }
    }
}
