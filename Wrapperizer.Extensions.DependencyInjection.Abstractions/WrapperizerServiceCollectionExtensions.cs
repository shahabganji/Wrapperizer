using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Wrapperizer.Extensions.DependencyInjection.Abstractions
{
    public static class WrapperizerServiceCollectionExtensions
    {
        public static IWrapperizerBuilder AddWrapperizer(this IServiceCollection serviceCollection)
        {
            return new WrapperizerBuilder(serviceCollection);
        }
    }
}
