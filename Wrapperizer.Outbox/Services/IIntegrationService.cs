using System;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Outbox.Services
{
    public interface IIntegrationService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent @event);
    }
}
