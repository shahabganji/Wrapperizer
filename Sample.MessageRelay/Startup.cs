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
using Microsoft.Extensions.Options;
using Sample.MessageRelay.BackgroundTasks;
using Sample.MessageRelay.Extensions;
using Wrapperizer;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Sample.Configurations;
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
            var sql = new SqlServerConnection();
            Configuration.Bind("Infra:Connections:Sql", sql);
            services.Configure<SqlServerConnection>(instance => Configuration.Bind("Infra:Connections:Sql", instance));
            services.AddScoped(x => x.GetRequiredService<IOptionsSnapshot<SqlServerConnection>>().Value);
            
            services.Configure<RabbitMqConnection>(instance => Configuration.Bind("Infra:Connections:RabbitMQ", instance));
            services.AddScoped(x => x.GetRequiredService<IOptionsSnapshot<RabbitMqConnection>>().Value);
            
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

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
            services.AddHostedService<MessageRelayWorker>();

            services.AddCustomHealthCheck(this.Configuration)
                .AddOptions();

            services.AddWrapperizer()
                .AddMessageRelayServices(
                    x =>
                    {
                        x.UseSqlServer(sql.ConnectionString);
                    });
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
