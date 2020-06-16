using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Definition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Wrapperizer;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Outbox;
using Wrapperizer.Sample.Configurations;

namespace Sample.Api
{
    public class Startup : GraceStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            
            services.AddOptionsAndHealthChecks(Configuration);

            services.AddMassTransitHostedService();
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();
                cfg.AddBus(factory => Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    x.Host("localhost", "wrapperizer");
                    x.ConfigureEndpoints(factory);
                }));
            });
        }

        public Startup(IConfiguration configuration) : base(configuration)
        {
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

            services.AddWrapperizer().AddOutboxServices(options =>
            {
                options.UseSqlServer(sql.ConnectionString,
                    sqlOptions =>
                    {
                        // sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

                        sqlOptions.MigrationsHistoryTable("__OutboxMigrationHistory", OutboxEventContext.DefaultSchema);
                        
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            }, true);
            
            return services;
        }
    }
    
}
