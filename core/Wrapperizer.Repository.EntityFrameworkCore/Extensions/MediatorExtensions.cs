using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Repository.EntityFrameworkCore.Extensions
{
    internal static class MediatorExtensions
    {
        internal static async Task DispatchDomainEventsAsync(this IMediator domainEventManager,
            DomainEventAwareDbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<IAggregateRoot>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await domainEventManager.Publish(domainEvent).ConfigureAwait(false);
        }
    }
}
