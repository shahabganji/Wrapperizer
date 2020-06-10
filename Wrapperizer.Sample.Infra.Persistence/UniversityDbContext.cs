using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore;

namespace Wrapperizer.Sample.Infra.Persistence
{
    public sealed class UniversityDbContext : DomainEventAwareDbContext
    {
        public UniversityDbContext(DbContextOptions options) : base(options)
        {
        }

        public UniversityDbContext(DbContextOptions options, IDomainEventManager domainEventManager,
            ILogger<DomainEventAwareDbContext> logger) : base(options, domainEventManager, logger)
        {
        }

    }
}
