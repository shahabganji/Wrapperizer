using System;
using System.Reflection;
using Funx.Extensions;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace Wrapperizer.Core
{
    public static class WrapperizerCoreServiceCollectionExtensions
    {
        public static WrapperizerCoreServiceCollection RegisterHandlers(
            this IWrapperizerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return builder.RegisterHandlers(assemblies);
        }

        public static WrapperizerCoreServiceCollection RegisterHandlers(
            this IWrapperizerBuilder builder,
            params Assembly[] assemblies)
            => RegisterHandlers(builder, null, assemblies);

        public static WrapperizerCoreServiceCollection RegisterHandlers(
            this IWrapperizerBuilder builder,
            Action<MediatRServiceConfiguration> configuration = null,
            params Assembly[] assemblies)
        {
            if (!assemblies.SafeAny())
                throw new ArgumentException(
                    "No assemblies found to scan. Supply at least one assembly to scan for handlers.");

            var mediatRServiceConfiguration =
                new MediatRServiceConfiguration()
                    .Using<Mediator>()
                    .AsScoped();
            configuration?.Invoke(mediatRServiceConfiguration);
            
            ServiceRegistrar.AddRequiredServices(builder.ServiceCollection, mediatRServiceConfiguration);
            ServiceRegistrar.AddMediatRClasses(builder.ServiceCollection, assemblies);

            var wrapperizerServiceCollection =
                new WrapperizerCoreServiceCollection(builder.ServiceCollection, mediatRServiceConfiguration);
            return wrapperizerServiceCollection;
        }

        public static WrapperizerCoreServiceCollection WithDomainEventManager(
            this WrapperizerCoreServiceCollection wrapperizerCore)
        {
            switch (wrapperizerCore.MediatRServiceConfiguration.Lifetime)
            {
                case Singleton:
                    wrapperizerCore.ServiceCollection
                        .AddSingleton<IDomainEventManager, DomainEventCommandQueryManager>();
                    break;
                case Scoped:
                    wrapperizerCore.ServiceCollection.AddScoped<IDomainEventManager, DomainEventCommandQueryManager>();
                    break;
                case Transient:
                    wrapperizerCore.ServiceCollection
                        .AddTransient<IDomainEventManager, DomainEventCommandQueryManager>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return wrapperizerCore;
        }

        public static WrapperizerCoreServiceCollection WithCommandQueryManager(
            this WrapperizerCoreServiceCollection wrapperizerCore)
        {
            switch (wrapperizerCore.MediatRServiceConfiguration.Lifetime)
            {
                case Singleton:
                    wrapperizerCore.ServiceCollection
                        .AddSingleton<ICommandQueryManager, DomainEventCommandQueryManager>();
                    break;
                case Scoped:
                    wrapperizerCore.ServiceCollection.AddScoped<ICommandQueryManager, DomainEventCommandQueryManager>();
                    break;
                case Transient:
                    wrapperizerCore.ServiceCollection
                        .AddTransient<ICommandQueryManager, DomainEventCommandQueryManager>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return wrapperizerCore;
        }

        public static void WithManagers(
            this WrapperizerCoreServiceCollection wrapperizerCore)
            =>
                wrapperizerCore.WithDomainEventManager()
                    .WithCommandQueryManager();
    }
}
