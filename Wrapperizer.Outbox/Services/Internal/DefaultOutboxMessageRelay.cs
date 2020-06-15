using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Outbox.Services.Internal
{
    internal sealed class DefaultOutboxMessageRelay : IOutboxMessageRelay
    {
        private readonly IOutboxEventService _outboxEventService;
        private readonly IBusControl _bus;
        private readonly ILogger<DefaultOutboxMessageRelay> _logger;

        public DefaultOutboxMessageRelay(
            IOutboxEventService outboxEventService, 
            ILogger<DefaultOutboxMessageRelay> logger, IBusControl bus)
        {
            _outboxEventService = outboxEventService;
            
            _logger = logger;
            _bus = bus;
        }
        
        public async Task PublishEventsThroughEventBusAsync()
        {
            var events = await _outboxEventService.RetrieveFailedEventsToPublishAsync();

            foreach (var @event in events)
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})",
                    @event.EventId, @event.IntegrationEvent);
                
                try
                {
                    await _outboxEventService.MarkEventAsInProgressAsync(@event.EventId);

                    await _bus.Publish(@event.IntegrationEvent, @event.IntegrationEvent.GetType());

                    await _outboxEventService.MarkEventAsPublishedAsync(@event.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId}",
                        @event.EventId);

                    await _outboxEventService.MarkEventAsFailedAsync(@event.EventId);
                }
            }
        }
    }
}
