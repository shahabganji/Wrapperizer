using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Serilog;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Extensions.Repositories.EfCore;
using Wrapperizer.Sample.Infra.Persistence;
using static HealthChecks.UI.Client.UIResponseWriter;

namespace Wrapperizer.Sample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHealthChecks()
                .AddCheck("api", _ => HealthCheckResult.Healthy())
                .AddRedis("localhost", "redis")
                .AddMongoDb("mongodb://127.0.0.1:27017" , "orderdb","mongodb")
                .AddSqlServer("Server=localhost; Database=fotokar; UID= sa; PWD=P@assw0rd")
                .AddRabbitMQ(new Uri("amqp://guest:guest@localhost:5672/wrapperizer"),new SslOption(), "rabbitmq");
            
            services.AddControllers();

            services.AddEntityFrameworkInMemoryDatabase();

            // services.AddDistributedMemoryCache();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "Wrapperizer.Api";
            });

            services.AddDbContext<UniversityDbContext>((provider,builder) =>
            {
                builder.UseInMemoryDatabase("sample_uni");
            });
           
            services.AddWrapperizer()
                .AddHandlers(context => context
                        .AddDistributedCaching()
                        .AddGlobalValidation()
                        .AddTransactionalCommands()
                )
                .AddUnitOfWork<UniversityDbContext>()
                .AddTransactionalUnitOfWork<UniversityDbContext>()
                // .AddCrudRepositories<WeatherForecastDbContext>((provider, options) =>
                // {
                //     options.UseInMemoryDatabase("WeatherForecast");
                //     options.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                // })
                ;

        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWrapperizerApiExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = WriteHealthCheckUIResponse 
                });
                
                endpoints.MapControllers();
            });
            
        }
    }
}
