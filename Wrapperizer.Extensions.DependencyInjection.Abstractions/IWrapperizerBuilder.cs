using Microsoft.Extensions.DependencyInjection;

namespace Wrapperizer.Extensions.DependencyInjection.Abstractions
{
    public interface IWrapperizerBuilder
    {
        IServiceCollection ServiceCollection { get; }
    }

    internal class WrapperizerBuilder : IWrapperizerBuilder
    {
        public WrapperizerBuilder(IServiceCollection serviceCollection) => ServiceCollection = serviceCollection;

        public IServiceCollection ServiceCollection { get; }
    }
}
