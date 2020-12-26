using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wrapperizer.Domain.Abstraction.Domain;

namespace Wrapperizer.Extensions.Cqrs.Events
{
    public class ValidationErrorHappened<T> : IDomainEvent
    {
        public Type Type { get; }
        public IReadOnlyCollection<ValidationResult> ValidationResults { get; }

        public ValidationErrorHappened(IReadOnlyCollection<ValidationResult> validationResults)
        {
            EventId = Guid.NewGuid();
            OccuredOn = DateTimeOffset.UtcNow;
            
            Type = typeof(T); 
            ValidationResults = validationResults;
        }

        public Guid EventId { get; }
        public DateTimeOffset OccuredOn { get; }
    }
}
