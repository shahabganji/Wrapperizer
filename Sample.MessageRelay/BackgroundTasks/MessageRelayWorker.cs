using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.IntegrationEvents;
using Wrapperizer.Outbox.Services;

namespace Sample.MessageRelay.BackgroundTasks
{
    public class MessageRelayWorker : BackgroundService
    {
        private readonly ILogger<MessageRelayWorker> _logger;
        private readonly IOutboxMessageRelay _outboxMessageRelay;

        public MessageRelayWorker(ILogger<MessageRelayWorker> logger, IOutboxMessageRelay outboxMessageRelay)
        {
            _logger = logger;
            _outboxMessageRelay = outboxMessageRelay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _outboxMessageRelay.PublishEventsThroughEventBusAsync();
                await Task.Delay(10_000, stoppingToken);
            }
        }
    }
}
