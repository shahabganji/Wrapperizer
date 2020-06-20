using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore;
using Wrapperizer.Extensions.Repositories.EfCore.Extensions;
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

        public void Foo()
        {
            var entities = from e in ChangeTracker.Entries()
                where e.State == EntityState.Added
                      || e.State == EntityState.Modified
                select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
            
            var migrationAssembly = typeof(UniversityDbContext).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(migrationAssembly);

            modelBuilder.AddAuditProperties(migrationAssembly);
            modelBuilder.AddSoftDeleteQueryFilter(migrationAssembly);

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
