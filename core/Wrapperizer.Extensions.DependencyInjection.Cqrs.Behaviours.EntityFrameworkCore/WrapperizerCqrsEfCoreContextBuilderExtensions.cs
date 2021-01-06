using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funx.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Cqrs.Behaviours.EntityFrameworkCore;
using Wrapperizer.Domain.Abstractions;
using Wrapperizer.Extensions.Common;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Extensions.DependencyInjection.Cqrs.Abstractions;

namespace Wrapperizer.Extensions.DependencyInjection.Cqrs.Behaviours.EntityFrameworkCore
{
    public static class WrapperizerCqrsEfCoreContextBuilderExtensions
    {
        public static CqrsContext AddTransaction(
            this CqrsContext context)
        {
            context.Services.Add(
                new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>),
                    context.ServiceLifetime));
            return context;
        }

        public static WrapperizerContext AddRepositories(this WrapperizerContext wrapperizerBuilder,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var repositories = GetRepositoryTypes();
            var services = wrapperizerBuilder.Services;

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
            public Type[] Interfaces { get; init; }
            public Type Implementation { get; init; }
        }
    }
}
