using System;
using System.Linq;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wrapperizer.AspNetCore.ServiceDiscovery.Consul;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;

namespace Wrapperizer
{
    public static class ServiceDiscoveryExtensions
    {
        public static IWrapperizerBuilder AddConsul(this IWrapperizerBuilder builder,
            Action<ConsulClientConfiguration> configurator)
        {
            var services = builder.ServiceCollection;
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(configurator));
            return builder;
        }

        public static IApplicationBuilder RegisterWithConsul(
            this IApplicationBuilder app,
            IHostApplicationLifetime lifetime,
            Action<ServiceRegistrationConfiguration, IServiceProvider> configurator = null
        )
        {
            if (lifetime == null) throw new ArgumentNullException(nameof(lifetime));

            // Retrieve Consul client from DI
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            // Setup logger
            var loggingFactory = app.ApplicationServices
                .GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            // Get server IP address
            var addresses = app.ServerFeatures.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();

            Console.WriteLine($"The api address is :{address}");
            logger.LogWarning($"The api address is :{address}");
            // Register service with consul
            var uri = new Uri(address);

            var serviceRegistration =
                app.ApplicationServices.GetService<IOptions<ServiceRegistrationConfiguration>>().Value;
            configurator?.Invoke(serviceRegistration, app.ApplicationServices);

            var health = new AgentCheckRegistration
            {
                HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceRegistration.HealthCheckEndpoint}",
                Notes = $"Checks {serviceRegistration.HealthCheckEndpoint} on {uri.Host}",
                Timeout = TimeSpan.FromSeconds(3),
                Interval = TimeSpan.FromSeconds(30),
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
            };

            var registration = new AgentServiceRegistration
            {
                // Checks = new AgentServiceCheck[] {health},

                ID = $"{serviceRegistration.ServiceId}-{uri.Port}",
                Name = serviceRegistration.ServiceName,
                Address = uri.Host,
                Port = uri.Port,
                Tags = serviceRegistration.Tags ?? new[] {serviceRegistration.ServiceName}
            };

            logger.LogInformation("Registering with Consul");

            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            // handle De-registration of the service
            // upon application shutdown, graceful shutdown not killing
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("De-registering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
