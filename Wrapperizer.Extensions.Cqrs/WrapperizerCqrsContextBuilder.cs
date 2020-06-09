using Microsoft.Extensions.DependencyInjection;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public sealed class WrapperizerCqrsContextBuilder
    {
        internal WrapperizerCqrsContextBuilder(IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            ServiceCollection = serviceCollection;
            ServiceLifetime = serviceLifetime;
        }

        public IServiceCollection ServiceCollection { get; }
        public ServiceLifetime ServiceLifetime { get; }
    }
}
