using System;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Abstraction;
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
                    wrapperizerServiceCollection.ServiceLifetime));

            // _ = serviceLifetime switch
            // {
            //     ServiceLifetime.Singleton => wrapperizerServiceCollection.ServiceCollection.AddSingleton(
            //         typeof(ICrudRepository<>), typeof(EfCoreCrudRepository<>)),
            //     ServiceLifetime.Scoped => wrapperizerServiceCollection.ServiceCollection.AddScoped(
            //         typeof(ICrudRepository<>), typeof(EfCoreCrudRepository<>)),
            //     _ => wrapperizerServiceCollection.ServiceCollection.AddTransient(
            //         typeof(ICrudRepository<>), typeof(EfCoreCrudRepository<>))
            // };

            return wrapperizerServiceCollection;
        }

        public static IWrapperizerBuilder AddUnitOfWork<T>(this IWrapperizerBuilder wrapperizerBuilder)
            where T : IUnitOfWork
        {
            wrapperizerBuilder.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(IUnitOfWork), typeof(T),
                    wrapperizerBuilder.ServiceLifetime));

            return wrapperizerBuilder;
        }

        public static IWrapperizerBuilder AddTransactionalUnitOfWork<T>(this IWrapperizerBuilder wrapperizerBuilder)
            where T : ITransactionalUnitOfWork
        {
            wrapperizerBuilder.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(ITransactionalUnitOfWork), typeof(T),
                    wrapperizerBuilder.ServiceLifetime));

            // _ = serviceLifeTime switch
            // {
            //     ServiceLifetime.Singleton => wrapperizerBuilder.ServiceCollection.AddSingleton(
            //         ),
            //     ServiceLifetime.Scoped => wrapperizerBuilder.ServiceCollection.AddScoped(
            //         typeof(ITransactionalUnitOfWork), typeof(T)),
            //     _ => wrapperizerBuilder.ServiceCollection.AddTransient(
            //         typeof(ITransactionalUnitOfWork), typeof(T))
            // };

            return wrapperizerBuilder;
        }
    }
}
