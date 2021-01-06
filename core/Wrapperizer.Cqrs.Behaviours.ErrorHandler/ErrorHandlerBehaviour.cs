using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Wrapperizer.Domain.Abstractions;
using static Wrapperizer.Domain.Abstractions.Result;

namespace Wrapperizer.Cqrs.Behaviours.ErrorHandler
{
    public sealed class ErrorHandlerBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, Result<TResponse>>
    {
        // private readonly IViewContext _viewContext;
        private readonly ILogger<ErrorHandlerBehaviour<TRequest, TResponse>> _logger;

        public ErrorHandlerBehaviour(
            ILogger<ErrorHandlerBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next)
        {
            if (next is null) throw new ArgumentNullException(nameof(next));

            try
            {
                return await next();
            }
            catch (Exception exception)
            {
                LogInnerException(exception);
                return Fail(exception.Message);
            }
        }

        private void LogInnerException(Exception exception)
        {
            if (exception.InnerException == null) return;

            _logger.LogError(exception, exception.Message);

            // may be an iteration be faster than method call??!!
            LogInnerException(exception.InnerException);
        }
    }
}
