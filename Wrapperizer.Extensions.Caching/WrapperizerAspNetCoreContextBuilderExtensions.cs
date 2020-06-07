using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Extensions.Caching.Behaviors;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class WrapperizerAspNetCoreContextBuilderExtensions
    {
        public static WrapperizerCqrsContextBuilder AddDistributedCaching(
            this WrapperizerCqrsContextBuilder context
        )
        {
            context.ServiceCollection.AddScoped(
                typeof(IPipelineBehavior<,>),
                typeof(CacheBehaviour<,>));
            return context;
        }
    }
}
