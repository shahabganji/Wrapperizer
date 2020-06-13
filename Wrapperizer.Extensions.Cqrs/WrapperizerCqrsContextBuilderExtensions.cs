using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Assembly[] GetListOfEntryAssemblyWithReferences()
        {
            var listOfAssemblies = new List<Assembly>();
            var mainAsm = Assembly.GetEntryAssembly();
            listOfAssemblies.Add(mainAsm);

            listOfAssemblies.AddRange(mainAsm.GetReferencedAssemblies().Select(Assembly.Load));
            return listOfAssemblies.ToArray();
        }
        
        public static IWrapperizerBuilder AddHandlers(
            this IWrapperizerBuilder builder,
            Action<WrapperizerCqrsContextBuilder> configure = null,
            ServiceLifetime serviceLifetime =  Transient,
            params Assembly[] assemblies)
        {
            if (!assemblies.SafeAny())
                assemblies = GetListOfEntryAssemblyWithReferences();

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
