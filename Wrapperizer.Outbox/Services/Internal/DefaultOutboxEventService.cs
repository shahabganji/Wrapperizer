using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Outbox.Services.Internal
{
    internal class DefaultOutboxEventService : IOutboxEventService
    {
        private readonly OutboxEventContext _outboxEventContext;
        private readonly List<Type> _eventTypes;

        public DefaultOutboxEventService(OutboxEventContext context)
        {
            // var dbConnection1 = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

            _outboxEventContext = context;

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => // t.Name.EndsWith(nameof(IntegrationEvent)) ||
                            typeof(IntegrationEvent).IsAssignableFrom(t))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrievePendingEventsToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            var result = await _outboxEventContext.Outbox
                .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished).ToListAsync();

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
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }
        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _outboxEventContext.Outbox.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if(status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _outboxEventContext.Outbox.Update(eventLogEntry);

            return _outboxEventContext.SaveChangesAsync();
        }
    }
}
