using Microsoft.Extensions.DependencyInjection;

namespace Wrapperizer.Extensions.DependencyInjection.Abstractions
{
    public class WrapperizerContext
    {
        public IServiceCollection Services { get; }

        internal WrapperizerContext(IServiceCollection services)
        {
            Services = services;
        }
    }
}
