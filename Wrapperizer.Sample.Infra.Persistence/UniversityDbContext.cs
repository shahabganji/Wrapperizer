using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Infra.Persistence
{
    public sealed class UniversityDbContext : DomainEventAwareDbContext
    {
        private const string DefaultSchema = "uni";
        
        public readonly Guid Id = Guid.NewGuid();
        
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
        {
        }

        public UniversityDbContext(DbContextOptions<UniversityDbContext> options, IDomainEventManager domainEventManager,
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

        public DbSet<Student> Students { get; set; }
        
    }
}
