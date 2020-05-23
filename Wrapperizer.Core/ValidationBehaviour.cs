using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funx.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Core.Abstraction.Specifications;

namespace Wrapperizer.Core
{
    public sealed class ValidationBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<Specification<TRequest>> _specifications;
        private readonly IActionResultAdapter _resultAdapter;

        public ValidationBehaviour(
            IEnumerable<Specification<TRequest>> specifications,
            IActionResultAdapter resultAdapter
            )
        {
            _specifications = specifications;
            _resultAdapter = resultAdapter;
        }

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

            _specifications.ForEach(spec =>
            {
                if (!spec.IsSatisfiedBy(request))
                {
                    validationResults.Add(new ValidationResult(spec.Error.Message));
                }
            });

            if (validationResults.Any())
            {
                _resultAdapter.Result = new BadRequestObjectResult(validationResults);
                return Task.FromResult<TResponse>(default);
            }
            
            return next();
        }
    }
}
