using System;
using Wrapperizer.Abstraction.Domain;

namespace Sample.IntegrationEvents
{
    public sealed class StudentRegisteredIntegrationEvent : IntegrationEvent
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; }

        public StudentRegisteredIntegrationEvent(Guid studentId, string fullName)
        {
            StudentId = studentId;
            FullName = fullName;
        }
    }
}
