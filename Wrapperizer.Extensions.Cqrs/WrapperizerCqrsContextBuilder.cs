using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public sealed class WrapperizerCqrsContextBuilder
    {
        internal WrapperizerCqrsContextBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get; }
    }
}
