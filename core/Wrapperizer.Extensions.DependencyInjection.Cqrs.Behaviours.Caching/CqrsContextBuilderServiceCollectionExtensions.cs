using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Cqrs.Behaviours.Caching;
using Wrapperizer.Extensions.DependencyInjection.Cqrs.Abstractions;

namespace Wrapperizer.Extensions.DependencyInjection.Cqrs.Behaviours.Caching
{
    public static class CqrsContextBuilderServiceCollectionExtensions
    {
        public static CqrsContext AddDistributedCache(this CqrsContext context)
        {
            context.Services.Add(new ServiceDescriptor(
                typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>),
                context.ServiceLifetime));

            return context;
        }
    }
}
