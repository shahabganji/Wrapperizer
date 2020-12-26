using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funx.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Extensions.Common;
using Wrapperizer.Extensions.Cqrs.EfCore.Behaviors;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerCqrsEfCoreContextBuilderExtensions
    {
        public static WrapperizerCqrsContextBuilder AddTransactionalCommands(
            this WrapperizerCqrsContextBuilder context)
        {
            context.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>),
                    context.ServiceLifetime));
            return context;
        }

        public static IWrapperizerBuilder AddRepositories(this IWrapperizerBuilder wrapperizerBuilder,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var repositories = GetRepositoryTypes();
            var services = wrapperizerBuilder.ServiceCollection;

            repositories.ForEach(definition =>
            {
                services.AddScoped(definition.Implementation);
                definition.Interfaces.ForEach(@interface =>
                {
                    services.AddScoped(@interface,
                        provider => provider.GetRequiredService(definition.Implementation));
                });
            });

            return wrapperizerBuilder;
        }

       
        private static IEnumerable<RepositoryDefinitions> GetRepositoryTypes()
        {
            var list = new List<RepositoryDefinitions>();
            foreach (var s in Assembly.GetEntryAssembly().WithReferencedAssemblies())
            foreach (var type in s.GetTypes())
            {
                if (!type.IsInterface && type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)))
                    list.Add(new RepositoryDefinitions
                    {
                        Implementation = type, Interfaces =
                            type.GetInterfaces()
                    });
            }

            return list;
        }

        private class RepositoryDefinitions
        {
            public Type[] Interfaces { get; set; }
            public Type Implementation { get; set; }
        }
    }
}
