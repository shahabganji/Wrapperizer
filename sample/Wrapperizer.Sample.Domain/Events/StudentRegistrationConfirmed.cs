using System;
using Wrapperizer.Domain.Abstraction.Domain;

namespace Wrapperizer.Sample.Domain.Events
{
    public class StudentRegistrationConfirmed : IDomainEvent
    {
        public Guid StudentId { get; private set; }

        public StudentRegistrationConfirmed(Guid studentId)
            => this.StudentId = studentId;
    }
}
