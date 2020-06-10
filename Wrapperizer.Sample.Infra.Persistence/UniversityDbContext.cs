using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore;

namespace Wrapperizer.Sample.Infra.Persistence
{
    public sealed class UniversityDbContext : DomainEventAwareDbContext
    {
        private const string DefaultSchema = "uni";
        
        public UniversityDbContext(DbContextOptions options) : base(options)
        {
        }

        public UniversityDbContext(DbContextOptions options, IDomainEventManager domainEventManager,
            ILogger<UniversityDbContext> logger) : base(options, domainEventManager, logger)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
            
            var migrationAssembly = typeof(UniversityDbContext).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(migrationAssembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("wrapperizer-sample");
            }
        }
    }
}
