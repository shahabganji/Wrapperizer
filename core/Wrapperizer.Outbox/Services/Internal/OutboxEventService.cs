using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Wrapperizer.Domain.Abstractions;
using Wrapperizer.Extensions.Common;
using static Wrapperizer.Outbox.EventStateEnum;

namespace Wrapperizer.Outbox.Services.Internal
{
    public class OutboxEventService : IOutboxEventService
    {
        private readonly OutboxEventContext _outboxEventContext;
        private readonly List<Type> _eventTypes;

        public OutboxEventService(OutboxEventContext context)
        {
            _outboxEventContext = context;

            var referencedAssemblies = Assembly.GetExecutingAssembly().WithEntryReferences();

            _eventTypes = referencedAssemblies.SelectMany(assembly =>
                assembly.GetTypes().Where(t => typeof(IntegrationEvent).IsAssignableFrom(t))
                    .ToList()
            ).ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrievePendingEventsToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            var result = await _outboxEventContext.Outbox
                .Where(e => e.TransactionId == tid && e.State == NotPublished).ToListAsync();

            if(result != null && result.Any()){
                return result.OrderBy(o => o.CreationTime)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t=> t.Name == e.EventTypeShortName)));
            }
            
            return new List<IntegrationEventLogEntry>();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveFailedEventsToPublishAsync()
        {
            var result = await _outboxEventContext.Outbox
                .Where(e => e.State == NotPublished || e.State == PublishedFailed).ToListAsync();

            if(result != null && result.Any()){
                return result.OrderBy(o => o.CreationTime)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t=> t.Name == e.EventTypeShortName)));
            }
            
            return new List<IntegrationEventLogEntry>();
        }
        
        public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

            _outboxEventContext.Database.UseTransaction(transaction.GetDbTransaction());
            _outboxEventContext.Outbox.Add(eventLogEntry);

            return _outboxEventContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, PublishedFailed);
        }
        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _outboxEventContext.Outbox.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if(status == InProgress)
                eventLogEntry.TimesSent++;

            _outboxEventContext.Outbox.Update(eventLogEntry);

            return _outboxEventContext.SaveChangesAsync();
        }
    }
}
