using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;
using Wrapperizer.Outbox.Services;

namespace Wrapperizer.Outbox.Services
{
    public sealed class DefaultIntegrationService : IIntegrationService
    {
        private readonly ITransactionalUnitOfWork _unitOfWork;

        private readonly ILogger<DefaultIntegrationService> _logger;
        private readonly IOutboxEventService _outboxEventService;

        public DefaultIntegrationService(
            ITransactionalUnitOfWork unitOfWork,
            Func<DbConnection, IOutboxEventService> integrationServiceFactory,
            ILogger<DefaultIntegrationService> logger
        )
        {
            _unitOfWork = unitOfWork;
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
                    // _eventBus.Publish(logEvt.IntegrationEvent);
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
