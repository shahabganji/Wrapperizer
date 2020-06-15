using HealthChecks.UI.Client;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sample.University.Notification.BackgroundTasks;
using Sample.University.Notification.Consumers;
using Sample.UniversityMessageRelay.BackgroundTasks.Extensions;
using Wrapperizer;

namespace Sample.University.Notification
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                
                cfg.AddConsumer<StudentRegisteredConsumer>();

                cfg.AddBus(factory => Bus.Factory.CreateUsingRabbitMq(
                    rabbit =>
                    {
                        rabbit.Host("localhost", "wrapperizer");
                        rabbit.ConfigureEndpoints(factory);
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
