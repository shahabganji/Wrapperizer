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
using Wrapperizer.Outbox.Services;
using Wrapperizer.Outbox.Utilities;

namespace Wrapperizer.Extensions.Cqrs.EfCore.Behaviors
{
    public sealed class TransactionBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ITransactionalUnitOfWork _transactionalUnitOfWork;
        private readonly ITransactionalOutboxService _transactionalOutboxService;
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

        public TransactionBehaviour(
            ITransactionalUnitOfWork transactionalUnitOfWork,
            ITransactionalOutboxService transactionalOutboxService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _transactionalUnitOfWork = transactionalUnitOfWork;
            _transactionalOutboxService = transactionalOutboxService;
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

                var strategy = _transactionalUnitOfWork.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    await using (var transaction =
                        await _transactionalUnitOfWork.BeginTransactionAsync(cancellationToken))
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})",
                            transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}",
                            transaction.TransactionId, typeName);

                        await _transactionalUnitOfWork.CommitTransactionAsync(transaction, cancellationToken);

                        transactionId = transaction.TransactionId;
                    }
                    // this is after the commit, so if it fails we can try them later on in a message relay, for instance.
                    await _transactionalOutboxService.PublishEventsThroughEventBusAsync(transactionId);
                });

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
