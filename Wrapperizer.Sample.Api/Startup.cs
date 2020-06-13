using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;
using Wrapperizer.Sample.Application;
using Wrapperizer.Sample.Configurations;
using Wrapperizer.Sample.Domain.Repositories;
using Wrapperizer.Sample.Infra.Persistence;
using Wrapperizer.Sample.Infra.Persistence.Repositories;
using static HealthChecks.UI.Client.UIResponseWriter;
using SqlServerConnection = Wrapperizer.Sample.Configurations.SqlServerConnection;

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
            services.AddOptionsAndHealthChecks(Configuration);
            
            services.AddControllers();
            
            services.AddOpenApiDocument(setting => setting.Title = "Sample Api");

            services.AddDbContext<UniversityDbContext>((provider,builder) =>
            {
                var sqlConnection = provider.GetRequiredService<SqlServerConnection>();

                builder.UseSqlServer(sqlConnection.ConnectionString, options =>
                {
                    options.EnableRetryOnFailure(3);
                });
            });
            
            var handlerAssembly = typeof(RegisterStudentHandler).Assembly;
            
            services.AddWrapperizer()
                .AddHandlers(context => context
                        .AddDistributedCaching()
                        .AddGlobalValidation()
                        .AddTransactionalCommands()
                , assemblies: new []{handlerAssembly})
                .AddUnitOfWork<UniversityDbContext>()
                .AddTransactionalUnitOfWork<UniversityDbContext>()
                // .AddCrudRepositories<WeatherForecastDbContext>((provider, options) =>
                // {
                //     options.UseInMemoryDatabase("WeatherForecast");
                //     options.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                // })
                ;

            services.AddScoped<IStudentRepository, StudentRepository>();

        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            
        }
    }

    public static class CustomExtensions
    {
        public static IServiceCollection AddOptionsAndHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            var sql = new SqlServerConnection();
            configuration.Bind("Infra:Connections:Sql", sql);
            services.Configure<SqlServerConnection>(instance => configuration.Bind("Infra:Connections:Sql", instance));
            services.AddScoped(x => x.GetRequiredService<IOptionsSnapshot<SqlServerConnection>>().Value);
            
            var mongodb = new MongoDbConnection();
            configuration.Bind("Infra:Connections:Mongodb", mongodb);
            services.Configure<MongoDbConnection>(instance => configuration.Bind("Infra:Connections:Mongodb", instance));
            services.AddScoped(x => x.GetRequiredService<IOptionsSnapshot<MongoDbConnection>>().Value);
            
            var rabbit = new RabbitMqConnection();
            configuration.Bind("Infra:Connections:RabbitMQ", rabbit);
            services.Configure<RabbitMqConnection>(instance => configuration.Bind("Infra:Connections:RabbitMQ", instance));
            services.AddScoped(x => x.GetRequiredService<IOptionsSnapshot<MongoDbConnection>>().Value);
            
            var redis = new RedisCacheOptions();
            configuration.Bind("Infra:Connections:Redis", redis);
            services.Configure<RedisCacheOptions>(instance => configuration.Bind("Infra:Connections:Redis", instance));
            services.AddScoped(x => x.GetRequiredService<IOptionsSnapshot<RedisCacheOptions>>().Value);
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "Wrapperizer.Api";
            });
            
            services.AddHealthChecks()
                .AddCheck("api", _ => HealthCheckResult.Healthy())
                .AddRedis(redis.Configuration, "redis")
                .AddMongoDb(mongodb.ConnectionString , mongodb.Collection,"mongodb")
                .AddSqlServer(sql.ConnectionString)
                .AddRabbitMQ(rabbit.ConnectionUri,new SslOption(), "rabbitmq");

            return services;
        }
    }
}
