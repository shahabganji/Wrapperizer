using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Outbox;
using Wrapperizer.Outbox.HostedServices;
using Wrapperizer.Outbox.Services;
using Wrapperizer.Outbox.Services.Internal;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class OutboxExtensions
    {
        public static IServiceCollection AddOutboxServices(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsBuilder , bool enableAutoMigration = true)
        {
            services.AddDbContext<OutboxEventContext>(optionsBuilder);

            services.AddTransient<Func<DbConnection, IOutboxEventService>>(
                sp => dbConnection => 
                    // this dbConnection will be passed on
                    // later on from implementations of integration service 
                {
                    var outboxEventContext = new OutboxEventContext(
                        new DbContextOptionsBuilder<OutboxEventContext>()
                            .UseSqlServer(dbConnection)
                            .Options);

                    return new DefaultOutboxEventService(outboxEventContext);
                });

            services.AddTransient<IIntegrationService, DefaultIntegrationService>();

            services.AddScoped<IOutboxMessageRelay, DefaultOutboxMessageRelay>();

            if (enableAutoMigration)
                services.AddHostedService<OutboxMigratorHostedService>();

            return services;
        }
        
        public static IServiceCollection AddMessageRelayServices(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsBuilder)
        {
            services.AddDbContext<OutboxEventContext>(optionsBuilder,Singleton,Singleton);
            services.AddSingleton<IOutboxEventService, DefaultOutboxEventService>();
            services.AddSingleton<IOutboxMessageRelay, DefaultOutboxMessageRelay>();

            return services;
        }
    }
}
