using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Extensions.Repositories.EfCore;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerRepositoryContextBuilderExtensions
    {
        public static IWrapperizerBuilder AddCrudRepositories<TU>
        (this IWrapperizerBuilder wrapperizerServiceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TU : DbContext
        {
            wrapperizerServiceCollection.ServiceCollection
                .AddDbContext<DbContext, TU>(optionsAction ?? throw new ArgumentNullException(nameof(optionsAction)));

            wrapperizerServiceCollection.ServiceCollection.Add(
                new ServiceDescriptor(typeof(ICrudRepository<>), typeof(EfCoreCrudRepository<>),
                    serviceLifetime));

            return wrapperizerServiceCollection;
        }

        public static IWrapperizerBuilder AddUnitOfWork<T>(this IWrapperizerBuilder wrapperizerBuilder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : IUnitOfWork
        {
            wrapperizerBuilder.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(IUnitOfWork),provider=> provider.GetRequiredService<T>(),
                    serviceLifetime));

            return wrapperizerBuilder;
        }

        public static IWrapperizerBuilder AddTransactionalUnitOfWork<T>(this IWrapperizerBuilder wrapperizerBuilder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : ITransactionalUnitOfWork
        {
            
            wrapperizerBuilder.ServiceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(IUnitOfWork), provider=> provider.GetRequiredService<T>(),
                    serviceLifetime));

            
            wrapperizerBuilder.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(ITransactionalUnitOfWork), provider=> provider.GetRequiredService<T>(),
                    serviceLifetime));

            return wrapperizerBuilder;
        }
    }
}
