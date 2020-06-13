using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Sample.Domain.Events;

namespace Wrapperizer.Sample.Api
{
    public sealed class StudentRegisteredHandler : IDomainEventHandler<StudentRegistered>
    {
        private readonly ILogger<StudentRegisteredHandler> _logger;

        public StudentRegisteredHandler(ILogger<StudentRegisteredHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task Handle(StudentRegistered notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event {EventName} handled by {HandlerName} for {@Event}" ,
                notification.GetType().Name, this.GetType().Name, notification
                );
        }
    }
}
