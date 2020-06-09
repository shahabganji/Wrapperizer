using System;
using System.Reflection;
using Funx.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Abstraction.Cqrs;
using Wrapperizer.Abstraction.Domain;
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
            Action<WrapperizerCqrsContextBuilder> configure = null,
            ServiceLifetime serviceLifetime =  Transient,
            params Assembly[] assemblies)
        {
            if (!assemblies.SafeAny())
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var wrapperizerCoreServiceCollection =
                new WrapperizerCqrsContextBuilder(builder.ServiceCollection, serviceLifetime);
            
            configure?.Invoke(wrapperizerCoreServiceCollection);

            builder.ServiceCollection.AddMediatR(assemblies,
                configuration => configuration = wrapperizerCoreServiceCollection.ServiceLifetime switch
                {
                    Singleton => configuration.AsSingleton(),
                    Scoped => configuration.AsScoped(),
                    _ => configuration.AsTransient()
                });

            builder.AddDomainEventManager<DomainEventCommandQueryManager>();
            builder.AddCommandQueryManger<DomainEventCommandQueryManager>();

            return builder;
        }

        public static WrapperizerCqrsContextBuilder AddGlobalValidation(this WrapperizerCqrsContextBuilder context)
        {
            context.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>),
                    context.ServiceLifetime));
            return context;
        }

        private static void AddCommandQueryManger<T>(this IWrapperizerBuilder builder,
            ServiceLifetime serviceLifetime = Transient)
            where T : ICommandQueryManager =>
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(ICommandQueryManager), typeof(DomainEventCommandQueryManager),
                    serviceLifetime));

        private static void AddDomainEventManager<T>(this IWrapperizerBuilder builder,
            ServiceLifetime serviceLifetime = Transient)
            where T : IDomainEventManager =>
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(IDomainEventManager), typeof(DomainEventCommandQueryManager),
                    serviceLifetime));
    }
}
