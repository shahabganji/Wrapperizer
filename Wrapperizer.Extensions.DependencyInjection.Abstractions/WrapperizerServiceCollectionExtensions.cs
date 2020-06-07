using System;
using Microsoft.Extensions.DependencyInjection;

namespace Wrapperizer.Extensions.DependencyInjection.Abstractions
{
    public static class WrapperizerServiceCollectionExtensions
    {
        public static IWrapperizerBuilder AddWrapperizer(
            this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new WrapperizerBuilder(serviceCollection,serviceLifetime);
        }
    }
}
