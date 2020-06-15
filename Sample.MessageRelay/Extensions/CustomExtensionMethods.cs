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
        
        public static ILoggingBuilder UseSerilog( this ILoggingBuilder builder, IConfiguration configuration)
        {
            // var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            // var logstashUrl = configuration["Serilog:LogstashgUrl"];

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", Program.AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                // .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                // .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return builder;
        }
    }
}
