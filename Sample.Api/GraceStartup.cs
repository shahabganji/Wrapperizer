using System;
using Grace.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wrapperizer;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Sample.Application.Handlers.Commands;
using Wrapperizer.Sample.Infra.Persistence;
using Wrapperizer.Sample.Infra.Persistence.AspNetCore.Migrator;
using static HealthChecks.UI.Client.UIResponseWriter;
using SqlServerConnection = Wrapperizer.Sample.Configurations.SqlServerConnection;

namespace Sample.Api
{
    public abstract class GraceStartup
    {
        protected IConfiguration Configuration { get; }
        protected GraceStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddOpenApiDocument(setting => setting.Title = "Sample Api");

            services.AddDbContext<UniversityDbContext>((provider,builder) =>
            {
                var sqlConnection = provider.GetRequiredService<SqlServerConnection>();

                builder.UseSqlServer(sqlConnection.ConnectionString, options =>
                {
                    options.EnableRetryOnFailure(3);
                });

                builder.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
            });

            var requestHandlersAssembly = typeof(RegisterStudentHandler).Assembly;
            var notificationHandlerAssembly = typeof(StudentRegisteredHandler).Assembly;

            services.AddWrapperizer()
                .AddHandlers(context => context
                        // .AddDistributedCaching()
                        // .AddGlobalValidation()
                        .AddTransactionalCommands()
                , assemblies:new []{requestHandlersAssembly,notificationHandlerAssembly}
                )
                .AddUnitOfWork<UniversityDbContext>()
                .AddTransactionalUnitOfWork<UniversityDbContext>()
                .AddRepositories()
                // .AddCrudRepositories<WeatherForecastDbContext>((provider, options) =>
                // {
                //     options.UseInMemoryDatabase("WeatherForecast");
                //     options.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                // })
                .AddConsul(consulConfig =>
                {
                    var address = this.Configuration["Infra:ServiceDiscovery:Address"];
                    consulConfig.Address = new Uri(address);                    
                });

            services.AddUniversityMigrator();

            // services.AddScoped<IStudentRepository, StudentRepository>();

        }
        
        public virtual void ConfigureContainer(IInjectionScope injectionScope)
        {
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWrapperizerApiExceptionHandler();

            app.UseHttpsRedirection();

            app.UseOpenApi();
            app.UseSwaggerUi3();

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
            
            app.RegisterWithConsul(lifetime , (configuration, provider) =>
            {
                this.Configuration.Bind("Infra:ServiceDiscovery:Consul", configuration);
                configuration.ServiceId = Guid.NewGuid().ToString();
            });
        }
    }
}
