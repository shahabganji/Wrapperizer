using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wrapperizer.Extensions.Cqrs.Exceptions
{
    public sealed class InvalidRequestException : Exception
    {
        public IReadOnlyCollection<ValidationResult> ValidationResults { get; }

        public InvalidRequestException(string message, IReadOnlyCollection<ValidationResult> validationResults)
            : base(message) {
            
            this.ValidationResults = validationResults;
        }
    }
}
