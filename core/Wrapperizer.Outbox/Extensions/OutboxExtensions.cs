using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
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
        private static bool _outboxServicesEnabled = false;
        private static bool _messageRelayServicesEnabled = false;

        private const string InvalidOperationExceptionMessage =
            "You should enable either 'MessageRelay' services or 'Outbox' services";

        public static IServiceCollection AddOutboxServices(
            this IWrapperizerBuilder builder,
            Action<DbContextOptionsBuilder> optionsBuilder , Action<TransactionalOutboxConfiguration> configure = null ,
            bool enableAutoMigration = true)
        {
            if( _messageRelayServicesEnabled )
                throw new InvalidOperationException(InvalidOperationExceptionMessage);

            var configuration = new TransactionalOutboxConfiguration {AutoPublish = false};
            configure?.Invoke(configuration);

            builder.ServiceCollection.AddSingleton(configuration);
            
            builder.ServiceCollection.AddDbContext<OutboxEventContext>(optionsBuilder);

            builder.ServiceCollection.AddTransient<Func<DbConnection, IOutboxEventService>>(
                sp => dbConnection => 
                    // this dbConnection will be passed on
                    // later on from implementations of integration service 
                {
                    var outboxEventContext = new OutboxEventContext(
                        new DbContextOptionsBuilder<OutboxEventContext>()
                            .UseSqlServer(dbConnection)
                            .Options);

                    return new OutboxEventService(outboxEventContext);
                });

            builder.ServiceCollection.AddTransient<ITransactionalOutboxService, TransactionalOutboxService>();

            builder.ServiceCollection.AddScoped<IOutboxMessageRelay, OutboxMessageRelay>();

            if (enableAutoMigration)
                builder.ServiceCollection.AddHostedService<OutboxMigratorHostedService>();

            return builder.ServiceCollection;
        }
        
        public static IServiceCollection AddMessageRelayServices(
            this IWrapperizerBuilder builder,
            Action<DbContextOptionsBuilder> optionsBuilder)
        {
            if (_outboxServicesEnabled)
                throw new InvalidOperationException(InvalidOperationExceptionMessage);

            builder.ServiceCollection.AddDbContext<OutboxEventContext>(optionsBuilder,Singleton,Singleton);
            builder.ServiceCollection.AddSingleton<IOutboxEventService, OutboxEventService>();
            builder.ServiceCollection.AddSingleton<IOutboxMessageRelay, OutboxMessageRelay>();

            return builder.ServiceCollection;
        }
    }
}
