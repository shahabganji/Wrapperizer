
using System;
using System.Threading.Tasks;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Outbox.Services
{
    public interface ITransactionalOutboxService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent @event);
    }
}
