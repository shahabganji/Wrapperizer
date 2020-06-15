using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.IntegrationEvents;
using Wrapperizer.Abstraction.Domain;

namespace Sample.University.Notification.Consumers
{
    public sealed class StudentRegisteredConsumer : IConsumer<StudentRegisteredIntegrationEvent>
    {
        private readonly ILogger<StudentRegisteredConsumer> _logger;

        public StudentRegisteredConsumer(ILogger<StudentRegisteredConsumer> logger)
        {
            _logger = logger;
        }


        public Task Consume(ConsumeContext<StudentRegisteredIntegrationEvent> context)
        {
            _logger.LogWarning("========== Message Received +==========================");
            _logger.LogInformation($"Sending notification to {context.Message.FullName}");
            _logger.LogWarning("========== Message Received +==========================");
            return Task.CompletedTask;
        }
    }

    public sealed class GlobalIntegrationEventConsumer : IConsumer<IntegrationEvent>
    {
        private readonly ILogger<GlobalIntegrationEventConsumer> _logger;

        public GlobalIntegrationEventConsumer(ILogger<GlobalIntegrationEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IntegrationEvent> context)
        {
            _logger.LogWarning("========== Message Received +==========================");
            _logger.LogInformation(
                $"Integration event occured at {context.Message.OccuredOn} received at {DateTime.UtcNow}");
            _logger.LogWarning("========== Message Received +==========================");
            return Task.CompletedTask;
        }
    }
}
