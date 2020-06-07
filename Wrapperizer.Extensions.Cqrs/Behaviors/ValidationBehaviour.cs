using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wrapperizer.Abstraction;
using Wrapperizer.Extensions.Cqrs.Events;
using Wrapperizer.Extensions.Cqrs.Exceptions;

namespace Wrapperizer.Extensions.Cqrs.Behaviors
{
    public sealed class ValidationBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IDomainEventManager _domainEventManager;

        public ValidationBehaviour(IDomainEventManager domainEventManager)
            => _domainEventManager = domainEventManager;

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var validationResults = new List<ValidationResult>();
            if (request is IValidatableObject validatableObject)
            {
                validationResults.AddRange(
                    validatableObject.Validate(
                        new ValidationContext(request)).ToList()
                );
            }

            if (!validationResults.Any()) return next();
            
            var validationErrorHappened = new ValidationErrorHappened<TRequest>(validationResults);
            _domainEventManager.Publish(validationErrorHappened);
                
            throw new InvalidRequestException($"Validations failed on the request object {nameof(TRequest)}",
                validationResults);
        }
    }
}
