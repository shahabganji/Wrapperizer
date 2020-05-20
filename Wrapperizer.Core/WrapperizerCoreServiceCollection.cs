using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Wrapperizer.Core
{
    public sealed class WrapperizerCoreServiceCollection : IWrapperizerServiceCollection
    {
        public IServiceCollection ServiceCollection { get; }

        internal MediatRServiceConfiguration MediatRServiceConfiguration { get; }

        internal WrapperizerCoreServiceCollection(
            IServiceCollection serviceCollection, MediatRServiceConfiguration mediatRServiceConfiguration)
        {
            ServiceCollection = serviceCollection;
            MediatRServiceConfiguration = mediatRServiceConfiguration;
        }
    }
}