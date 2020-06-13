using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Outbox.Services
{
    public interface IOutboxEventService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrievePendingEventsToPublishAsync(Guid transactionId);
        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
