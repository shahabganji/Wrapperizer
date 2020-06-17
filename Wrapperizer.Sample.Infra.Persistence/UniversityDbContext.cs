using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Funx.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Common;
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

            // AddShadowProperties(modelBuilder, migrationAssembly);
            AddSofDeleteQueryFilter(modelBuilder,migrationAssembly);
        }

        private static void AddShadowProperties(ModelBuilder modelBuilder, Assembly migrationAssembly)
        {
            var auditableTypes = migrationAssembly.WithReferencedAssemblies().SelectMany(assembly =>
                assembly.GetTypes().Where(type =>
                        typeof(ICanBeAudited).IsAssignableFrom(type)
                        && type.IsClass && !type.IsAbstract
                    )
                    .Select(type => type));

            foreach (var auditableType in auditableTypes)
            {
                modelBuilder.Entity(auditableType, builder => { builder.Property<DateTime>("UpdatedOn"); });
            }
        }

        private static void AddSofDeleteQueryFilter(ModelBuilder modelBuilder, Assembly migrationAssembly)
        {
            var softDeletedEntityTypes = migrationAssembly.WithReferencedAssemblies().SelectMany(assembly =>
                assembly.GetTypes().Where(type =>
                        typeof(ICanBeAudited).IsAssignableFrom(type)
                        && type.IsClass && !type.IsAbstract
                    )
                    .Select(type => type));

            softDeletedEntityTypes.ForEach(entityType =>
            {
                modelBuilder.Entity(entityType).HasQueryFilter(ConvertFilterExpression(entityType));
            });
        }
        
    
        
        private static LambdaExpression ConvertFilterExpression(Type entityType)
        {
            // // e => e.SoftDeleted == false
            // var value = Expression.Constant(false);
            // var parameter = Expression.Parameter(entityType, "e");
            // var property = Expression.Property(parameter, $"{nameof(ICanBeSoftDeleted.SoftDeleted)}");
            // var equal = Expression.Equal(property, value);
            // var lambda = Expression.Lambda(equal, parameter);
            
            // e => !e.SoftDeleted
            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, $"{nameof(ICanBeSoftDeleted.SoftDeleted)}");
            var notUnaryExpression = Expression.Not(property);
            var lambda = Expression.Lambda(notUnaryExpression, parameter);

            return lambda;
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
