using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Sample.MessageRelay.Extensions
{
    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            // hcBuilder.AddSqlServer(
            //         configuration["ConnectionString"],
            //         name: "UniversityTaskDB-check",
            //         tags: new string[] { "universitytaskdb" });
            //
            //
            //     hcBuilder.AddRabbitMQ(
            //             $"amqp://{configuration["EventBusConnection"]}",
            //             name: "university-task-rabbitmqbus-check",
            //             tags: new string[] { "rabbitmqbus" });
            

            return services;
        }
        
    }
}
