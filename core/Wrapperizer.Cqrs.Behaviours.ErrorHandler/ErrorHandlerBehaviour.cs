using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Behaviours.ErrorHandler
{
    public sealed class ErrorHandlerBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, ViewResult<TResponse>>
    {
        // private readonly IViewContext _viewContext;
        private readonly ILogger<ErrorHandlerBehaviour<TRequest, TResponse>> _logger;

        public ErrorHandlerBehaviour(
            ILogger<ErrorHandlerBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<ViewResult<TResponse>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<ViewResult<TResponse>> next)
        {
            if (next is null) throw new ArgumentNullException(nameof(next));
        
            var failedResult = ViewResult.Fail();
        
            try
            {
                var result = await next();
        
                return result;
            } catch (Exception exception)
            {
                LogInnerException(exception);
                failedResult.Fail(exception.Message);
            }
        
            return failedResult;
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
