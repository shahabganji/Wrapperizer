using Microsoft.Extensions.DependencyInjection;

namespace Wrapperizer.Extensions.DependencyInjection.Abstractions
{
    public interface IWrapperizerBuilder
    {
        IServiceCollection ServiceCollection { get; }

        ServiceLifetime ServiceLifetime { get; }
    }

    internal class WrapperizerBuilder : IWrapperizerBuilder
    {
        public WrapperizerBuilder(IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            ServiceCollection = serviceCollection;
            ServiceLifetime = serviceLifetime;
        }

        public IServiceCollection ServiceCollection { get; }
        public ServiceLifetime ServiceLifetime { get; }
    }
}
