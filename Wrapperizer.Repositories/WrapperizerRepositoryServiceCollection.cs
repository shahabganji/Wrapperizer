using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;

namespace Wrapperizer.Repositories
{
    public sealed class WrapperizerRepositoryServiceCollection : IWrapperizerBuilder
    {
        public WrapperizerRepositoryServiceCollection(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get; }
    }
}
