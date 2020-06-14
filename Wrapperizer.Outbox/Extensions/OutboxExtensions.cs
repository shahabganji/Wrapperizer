using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Outbox;
using Wrapperizer.Outbox.HostedServices;
using Wrapperizer.Outbox.Services;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class OutboxExtensions
    {

        public static IServiceCollection AddOutboxServices(this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsBuilder , bool enableAutoMigration = true)
        {
            services.AddDbContext<OutboxEventContext>(optionsBuilder);

            services.AddTransient<Func<DbConnection, IOutboxEventService>>(
                sp => dbConnection => // this dbConnection will be passed on
                    // later on from implementations of integration service 
                {
                    var outboxEventContext = new OutboxEventContext(
                        new DbContextOptionsBuilder<OutboxEventContext>()
                            .UseSqlServer(dbConnection)
                            .Options);

                    return new OutboxEventService(outboxEventContext);
                });

            services.AddTransient<IIntegrationService, DefaultIntegrationService>();

            if (enableAutoMigration)
                services.AddHostedService<OutboxMigratorHostedService>();

            return services;
        }
    }
}
