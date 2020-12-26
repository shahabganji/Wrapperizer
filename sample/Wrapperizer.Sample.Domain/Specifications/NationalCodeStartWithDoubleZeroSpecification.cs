using System;
using System.Linq.Expressions;
using Wrapperizer.Domain.Abstraction.Specifications;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Domain.Specifications
{
    public class NationalCodeStartWithDoubleZeroSpecification : Specification<Student>
    {
        public override Expression<Func<Student, bool>> ToExpression()
        {
            // bool Filter(Student student)
            // {
            //     return student.NationalCode.ToString().StartsWith("00");
            // }
            return student => student.NationalCode.ToString().StartsWith("00");
        }
    }
}
