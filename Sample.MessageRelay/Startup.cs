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
using Sample.MessageRelay.BackgroundTasks;
using Sample.MessageRelay.Extensions;
using Wrapperizer;
using MassTransitHostedService = Sample.MessageRelay.BackgroundTasks.MassTransitHostedService;

namespace Sample.MessageRelay
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
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.AddBus(factory => Bus.Factory.CreateUsingRabbitMq(
                    rabbit =>
                    {
                        rabbit.Host("localhost", "wrapperizer");
                        rabbit.ConfigureEndpoints(factory);
                    })
                );
            });

            services.AddHostedService<MassTransitHostedService>();
            services.AddHostedService<MessageRelayWorker>();

            services.AddCustomHealthCheck(this.Configuration)
                .AddOptions();

            services.AddMessageRelayServices(
                x => { x.UseSqlServer("Server=localhost; UID=sa; PWD=P@assw0rd; Database=WrapperizeR"); });
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
