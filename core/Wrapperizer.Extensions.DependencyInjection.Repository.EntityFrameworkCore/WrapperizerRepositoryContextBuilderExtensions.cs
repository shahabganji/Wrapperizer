using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wrapperizer.Domain.Abstractions;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Repository.EntityFrameworkCore;
using Wrapperizer.Repository.EntityFrameworkCore.Abstraction;

namespace Wrapperizer.Extensions.DependencyInjection.Repository.EntityFrameworkCore
{
    public static class WrapperizerRepositoryContextBuilderExtensions
    {
        public static WrapperizerContext AddModelDbContext<TIContext,TDbContext>(this WrapperizerContext ringanaContext,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped , 
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TDbContext : DbContext
        {

            if (ringanaContext == null) throw new ArgumentNullException(nameof(ringanaContext));

            ringanaContext.Services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);
            ringanaContext.Services.Add(new ServiceDescriptor(typeof(TIContext) , typeof(TDbContext), contextLifetime));


            return ringanaContext;
        }

        public static WrapperizerContext AddModelDbContext<TIContext, TDbContext>(this WrapperizerContext ringanaContext,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped , 
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TDbContext : DbContext
        {

            if (ringanaContext == null) throw new ArgumentNullException(nameof(ringanaContext));

            ringanaContext.Services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);
            ringanaContext.Services.Add(new ServiceDescriptor(typeof(TIContext) , typeof(TDbContext), contextLifetime));
            
            return ringanaContext;
        }
        public static WrapperizerContext AddCrudRepositories<TU>
        (this WrapperizerContext wrapperizerContext,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TU : DbContext
        {
            wrapperizerContext.Services
                .AddDbContext<DbContext, TU>(optionsAction ?? throw new ArgumentNullException(nameof(optionsAction)));

            wrapperizerContext.Services.Add(
                new ServiceDescriptor(typeof(ICrudRepository<>), typeof(EfCoreCrudRepository<>),
                    serviceLifetime));

            return wrapperizerContext;
        }

        public static WrapperizerContext AddUnitOfWork<T>(this WrapperizerContext wrapperizerBuilder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : IUnitOfWork
        {
            wrapperizerBuilder.Services.Add(
                new ServiceDescriptor(
                    typeof(IUnitOfWork),provider=> provider.GetRequiredService<T>(),
                    serviceLifetime));

            return wrapperizerBuilder;
        }

        public static WrapperizerContext AddTransactionalUnitOfWork<T>(this WrapperizerContext wrapperizerBuilder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : ITransactionalUnitOfWork
        {
            
            wrapperizerBuilder.Services.TryAdd(
                new ServiceDescriptor(
                    typeof(IUnitOfWork), provider=> provider.GetRequiredService<T>(),
                    serviceLifetime));

            
            wrapperizerBuilder.Services.Add(
                new ServiceDescriptor(
                    typeof(ITransactionalUnitOfWork), provider=> provider.GetRequiredService<T>(),
                    serviceLifetime));

            return wrapperizerBuilder;
        }
    }
}
