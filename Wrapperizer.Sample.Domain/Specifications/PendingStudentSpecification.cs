using System;
using System.Linq.Expressions;
using Wrapperizer.Abstraction.Specifications;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Domain.Specifications
{
    public class PendingStudentSpecification : Specification<Student>
    {
        public override Expression<Func<Student, bool>> ToExpression()
        {
            return (student) => 
                student.RegistrationStatus == RegistrationStatus.Requested;
        }
    }
}
