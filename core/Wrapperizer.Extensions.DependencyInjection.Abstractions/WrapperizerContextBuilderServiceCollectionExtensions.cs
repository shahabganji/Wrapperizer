using Microsoft.Extensions.DependencyInjection;

namespace Wrapperizer.Extensions.DependencyInjection.Abstractions
{
    public static class WrapperizerContextBuilderServiceCollectionExtensions
    {
        public static WrapperizerContext AddWrapperizer(this IServiceCollection services)
        {
            var ringanaContext = new WrapperizerContext(services);
            
            services.AddSingleton(ringanaContext);

            return ringanaContext;
        }
    }
}
