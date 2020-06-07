using Microsoft.Extensions.DependencyInjection;

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
