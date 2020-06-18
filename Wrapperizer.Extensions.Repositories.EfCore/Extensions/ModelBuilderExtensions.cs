using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Funx.Extensions;
using Microsoft.EntityFrameworkCore;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Common;

namespace Wrapperizer.Extensions.Repositories.EfCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void AddSoftDeleteQueryFilter(this ModelBuilder modelBuilder, Assembly migrationAssembly)
        {
            var softDeletedEntityTypes = migrationAssembly.WithReferencedAssemblies().SelectMany(assembly =>
                assembly.GetTypes().Where(type =>
                        typeof(ICanBeAudited).IsAssignableFrom(type)
                        && type.IsClass && !type.IsAbstract
                    )
                    .Select(type => type));

            softDeletedEntityTypes.ForEach(entityType =>
            {
                modelBuilder.Entity((Type) entityType).HasQueryFilter(GenerateQueryFilterExpression(entityType));
            });
        }

        private static void AddShadowProperties(this ModelBuilder modelBuilder, Assembly migrationAssembly)
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
        private static LambdaExpression GenerateQueryFilterExpression(Type entityType)
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
    }
}
