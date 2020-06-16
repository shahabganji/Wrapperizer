using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using GreenPipes.Internals.Extensions;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;

namespace Wrapperizer.Outbox.Services.Internal
{
    internal sealed class TransactionalOutboxService : ITransactionalOutboxService
    {
        private readonly ITransactionalUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;


        private readonly ILogger<TransactionalOutboxService> _logger;
        private readonly IOutboxEventService _outboxEventService;

        public TransactionalOutboxService(
            ITransactionalUnitOfWork unitOfWork,
            Func<DbConnection, IOutboxEventService> integrationServiceFactory,
            IPublishEndpoint publishEndpoint,
            ILogger<TransactionalOutboxService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;

            _logger = logger;
            _outboxEventService = integrationServiceFactory(unitOfWork.GetDbConnection());
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _outboxEventService.RetrievePendingEventsToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})",
                    logEvt.EventId, logEvt.IntegrationEvent);

                try
                {
                    await _outboxEventService.MarkEventAsInProgressAsync(logEvt.EventId);

                    await _publishEndpoint.Publish(logEvt.IntegrationEvent, logEvt.IntegrationEvent.GetType());

                    await _outboxEventService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId}",
                        logEvt.EventId);

                    await _outboxEventService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent @event)
        {
            _logger.LogInformation(
                "----- Enqueuing integration event {IntegrationEventId} to Outbox ({@IntegrationEvent})", @event.Id,
                @event);

            await _outboxEventService.SaveEventAsync(@event, _unitOfWork.GetCurrentTransaction());
        }
    }
}
