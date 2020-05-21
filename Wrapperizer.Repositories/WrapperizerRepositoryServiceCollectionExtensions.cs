using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Repositories;
using Wrapperizer.Repositories.Abstraction;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerRepositoryServiceCollectionExtensions
    {
        public static IWrapperizerBuilder AddCrudRepositories
        (this IWrapperizerBuilder wrapperizerServiceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
        {
            wrapperizerServiceCollection.ServiceCollection
                .AddDbContext<DbContext>(optionsAction ?? throw new ArgumentNullException(nameof(optionsAction)));

            wrapperizerServiceCollection.ServiceCollection.AddScoped(
                typeof(ICrudRepository<>), typeof(EfCoreCrudRepository<>));
            return wrapperizerServiceCollection;
        }

        public static IWrapperizerBuilder AddUnitOfWork<T>(
            this IWrapperizerBuilder wrapperizerBuilder)
            where T : IUnitOfWork
        {
            wrapperizerBuilder.ServiceCollection
                .AddScoped(typeof(IUnitOfWork), typeof(T));
            return wrapperizerBuilder;
        }

        public static IWrapperizerBuilder AddTransactionalUnitOfWork<T>(
            this IWrapperizerBuilder wrapperizerBuilder)
            where T : ITransactionalUnitOfWork
        {
            wrapperizerBuilder.ServiceCollection
                .AddScoped(typeof(ITransactionalUnitOfWork), typeof(T));
            return wrapperizerBuilder;
        }
    }
}
