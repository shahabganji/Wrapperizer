using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Extensions.Cqrs.Caching.Behaviors;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerAspNetCoreContextBuilderExtensions
    {
        public static WrapperizerCqrsContextBuilder AddDistributedCaching(
            this WrapperizerCqrsContextBuilder context)
        {
            context.ServiceCollection.Add(
                new ServiceDescriptor(
                    typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>)
                    , context.ServiceLifetime));
            return context;
        }
    }
}
