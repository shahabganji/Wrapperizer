namespace Wrapperizer.Extensions.AspNetCore.ServiceDiscovery.Consul
{
    public sealed class ServiceRegistrationConfiguration
    {
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string[] Tags { get; set; }
        public string HealthCheckEndpoint { get; set; }
    }
}
