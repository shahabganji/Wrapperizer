using System.Linq;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Extensions.Repositories.EfCore.Extensions
{
    internal static class MediatorExtensions
    {
        internal static async Task DispatchDomainEventsAsync(this IDomainEventManager domainEventManager,
            DomainEventAwareDbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<IEntity>()
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
