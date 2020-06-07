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
            Action<WrapperizerCqrsContextBuilder> configure = null,
            params Assembly[] assemblies)
        {
            if (!assemblies.SafeAny())
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.ServiceCollection.AddMediatR(assemblies,
                configuration => configuration = builder.ServiceLifetime switch
                {
                    Singleton => configuration.AsSingleton(),
                    Scoped => configuration.AsScoped(),
                    _ => configuration.AsTransient()
                });

            var wrapperizerCoreServiceCollection = new WrapperizerCqrsContextBuilder(
                builder.ServiceCollection, builder.ServiceLifetime);
            configure?.Invoke(wrapperizerCoreServiceCollection);

            builder.AddDomainEventManager<DomainEventCommandQueryManager>();
            builder.AddCommandQueryManger<DomainEventCommandQueryManager>();

            return builder;
        }

        public static WrapperizerCqrsContextBuilder AddGlobalValidation(this WrapperizerCqrsContextBuilder context)
        {
            context.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>),
                    typeof(ValidationBehaviour<,>), context.ServiceLifetime));
            return context;
        }

        private static IWrapperizerBuilder AddCommandQueryManger<T>(this IWrapperizerBuilder builder)
            where T : ICommandQueryManager
        {
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(ICommandQueryManager),
                    typeof(DomainEventCommandQueryManager), builder.ServiceLifetime));

            return builder;
        }

        private static IWrapperizerBuilder AddDomainEventManager<T>(
            this IWrapperizerBuilder builder)
            where T : IDomainEventManager
        {
            builder.ServiceCollection.Add(
                new ServiceDescriptor(typeof(IDomainEventManager),
                    typeof(DomainEventCommandQueryManager),
                    builder.ServiceLifetime));

            return builder;
        }
    }
}
