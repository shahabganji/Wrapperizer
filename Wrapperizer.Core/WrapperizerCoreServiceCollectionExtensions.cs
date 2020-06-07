using System;
using System.Reflection;
using Funx.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Core;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{

    public sealed class WrapperizerCoreServiceCollection
    {
        internal WrapperizerCoreServiceCollection(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        internal  IServiceCollection ServiceCollection { get; }
    }

    public sealed class HandlersConfiguration
    {
        public HandlersConfiguration(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        public ServiceLifetime Lifetime { get; }
    }
    
    public static class WrapperizerCoreServiceCollectionExtensions
    {
        public static IWrapperizerBuilder AddHandlers(
            this IWrapperizerBuilder builder,
            ServiceLifetime serviceLifetime = Transient,
            Action<WrapperizerCoreServiceCollection> configure = null,
            params Assembly[] assemblies)
        {
            builder.ServiceCollection.AddScoped<IActionResultAdapter, ActionResultAdapter>();
            
            if( !assemblies.SafeAny() )
                assemblies =  AppDomain.CurrentDomain.GetAssemblies();
            
            builder.ServiceCollection.AddMediatR(assemblies, 
                configuration => configuration = serviceLifetime switch
            {
                Singleton => configuration.AsSingleton(),
                Scoped => configuration.AsScoped(),
                _ => configuration.AsTransient()
            });

            var wrapperizerCoreServiceCollection = new WrapperizerCoreServiceCollection(builder.ServiceCollection);
            configure?.Invoke(wrapperizerCoreServiceCollection);

            builder.AddDomainEventManager<DomainEventCommandQueryManager>(serviceLifetime);
            builder.AddCommandQueryManger<DomainEventCommandQueryManager>(serviceLifetime);

            return builder;
        }

        public static WrapperizerCoreServiceCollection AddGlobalCaching(this WrapperizerCoreServiceCollection wsc)
        {
            wsc.ServiceCollection.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(CacheBehaviour<,>));
            return wsc;
        }
        
        public static WrapperizerCoreServiceCollection AddGlobalValidation(this WrapperizerCoreServiceCollection wsc)
        {
            wsc.ServiceCollection.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehaviour<,>));
            return wsc;
        }

        public static IWrapperizerBuilder AddCommandQueryManger<T>(this IWrapperizerBuilder builder, ServiceLifetime serviceLifetime)
            where T : ICommandQueryManager
        {
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(ICommandQueryManager),
                    typeof(DomainEventCommandQueryManager), serviceLifetime));

            return builder;
        }

        public static IWrapperizerBuilder AddDomainEventManager<T>(
            this IWrapperizerBuilder builder, ServiceLifetime serviceLifetime = Transient)
            where T : IDomainEventManager
        {
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(IDomainEventManager),
                    typeof(DomainEventCommandQueryManager), serviceLifetime));
            return builder;
        }
    }
}
