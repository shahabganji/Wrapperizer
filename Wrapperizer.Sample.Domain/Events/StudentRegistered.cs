using System;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Domain.Events
{
    public sealed class StudentRegistered : IDomainEvent
    {
        public Student Student { get; set; }

        public StudentRegistered(Student student)
            => this.Student = student;
    }
}
