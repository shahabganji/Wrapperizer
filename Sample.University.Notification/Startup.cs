using HealthChecks.UI.Client;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.University.Notification.BackgroundTasks;
using Sample.University.Notification.Consumers;
using Sample.University.Notification.Extensions;
using Wrapperizer.Sample.Configurations;


namespace Sample.University.Notification
{
    public sealed class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RabbitMqConnection>(instance => Configuration.Bind("Infra:Connections:RabbitMQ", instance));
            services.AddScoped(x => x.GetRequiredService<IOptionsSnapshot<MongoDbConnection>>().Value);


            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                
                cfg.AddConsumer<StudentRegisteredConsumer>();

                cfg.AddBus(factory => Bus.Factory.CreateUsingRabbitMq(
                    host =>
                    {
                        var rabbit = factory.Container.GetRequiredService<RabbitMqConnection>();
                        host.UseHealthCheck(factory);
                        host.Host(rabbit.Host, rabbit.VirtualHost);
                        host.ConfigureEndpoints(factory);
                    })
                );
            });

            services.AddHostedService<MassTransitHostedService>();

            services.AddCustomHealthCheck(this.Configuration)
                .AddOptions();
        }


        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}
