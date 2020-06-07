using System;
using System.Reflection;
using Funx.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Abstraction;
using Wrapperizer.Extensions.Cqrs.Behaviors;
using Wrapperizer.Extensions.Cqrs.Managers;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerCqrsContextBuilderExtensions
    {
        public static IWrapperizerBuilder AddHandlers(
            this IWrapperizerBuilder builder,
            ServiceLifetime serviceLifetime = Transient,
            Action<WrapperizerCqrsContextBuilder> configure = null,
            params Assembly[] assemblies)
        {
            if( !assemblies.SafeAny() )
                assemblies =  AppDomain.CurrentDomain.GetAssemblies();
            
            builder.ServiceCollection.AddMediatR(assemblies, 
                configuration => configuration = serviceLifetime switch
            {
                Singleton => configuration.AsSingleton(),
                Scoped => configuration.AsScoped(),
                _ => configuration.AsTransient()
            });

            var wrapperizerCoreServiceCollection = new WrapperizerCqrsContextBuilder(builder.ServiceCollection);
            configure?.Invoke(wrapperizerCoreServiceCollection);

            builder.AddDomainEventManager<DomainEventCommandQueryManager>(serviceLifetime);
            builder.AddCommandQueryManger<DomainEventCommandQueryManager>(serviceLifetime);

            return builder;
        }

        public static WrapperizerCqrsContextBuilder AddGlobalValidation(this WrapperizerCqrsContextBuilder context)
        {
            context.ServiceCollection.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehaviour<,>));
            return context;
        }

        private static IWrapperizerBuilder AddCommandQueryManger<T>(this IWrapperizerBuilder builder, ServiceLifetime serviceLifetime)
            where T : ICommandQueryManager
        {
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(ICommandQueryManager),
                    typeof(DomainEventCommandQueryManager), serviceLifetime));

            return builder;
        }

        private static IWrapperizerBuilder AddDomainEventManager<T>(
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
