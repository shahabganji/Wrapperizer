using Microsoft.Extensions.DependencyInjection;

namespace Wrapperizer.Extensions.DependencyInjection.Cqrs.Abstractions
{
    public class CqrsContext
    {
        public IServiceCollection Services { get; }
        public ServiceLifetime ServiceLifetime { get; }

        internal CqrsContext(IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            Services = services;
            ServiceLifetime = serviceLifetime;
        }
    }
}
