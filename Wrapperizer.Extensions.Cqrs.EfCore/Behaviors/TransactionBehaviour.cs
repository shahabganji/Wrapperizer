using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Wrapperizer.Abstraction.Cqrs;
using Wrapperizer.Extensions.Cqrs.EfCore.Extensions;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;

namespace Wrapperizer.Extensions.Cqrs.EfCore.Behaviors
{
    public sealed class TransactionBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ITransactionalUnitOfWork _transactionalUnitOfWork;
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

        public TransactionBehaviour(
            ITransactionalUnitOfWork transactionalUnitOfWork, 
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _transactionalUnitOfWork = transactionalUnitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (command is ICommand<TResponse>)
            {
                return await HandleTransactionalRequest(command, next, cancellationToken);
            }

            return await next();
        }
        
        private async Task<TResponse> HandleTransactionalRequest(TRequest request,
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_transactionalUnitOfWork.TransactionInProgress)
                {
                    return await next();
                }

                await _transactionalUnitOfWork.ExecuteTransactionAsync(async (transaction) =>
                {
                    using var disposable = LogContext.PushProperty("TransactionContext", transaction.TransactionId);
                    _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})",
                        transaction.TransactionId, typeName, request);

                    response = await next();

                    _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}",
                        transaction.TransactionId, typeName);
                }, cancellationToken);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                return response;
            }
        }
    }
}
