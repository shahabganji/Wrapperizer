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

namespace Wrapperizer.Extensions.DependencyInjection.Outbox
{
    public static class OutboxServiceCollectionExtensions
    {
        private static readonly bool _outboxServicesEnabled = false;
        private static readonly bool _messageRelayServicesEnabled = false;

        private static readonly string InvalidOperationExceptionMessage =
            $"You should enable either 'MessageRelay' services or 'Outbox' services; respectively using " +
            $"{nameof(AddMessageRelayServices)} or {nameof(AddOutboxServices)}";

        public static WrapperizerContext AddOutboxServices(
            this WrapperizerContext builder,
            Action<DbContextOptionsBuilder> optionsBuilder, Action<TransactionalOutboxConfiguration> configure = null,
            bool enableAutoMigration = true)
        {
            if (_messageRelayServicesEnabled)
                throw new InvalidOperationException(InvalidOperationExceptionMessage);

            var configuration = new TransactionalOutboxConfiguration {AutoPublish = false};
            configure?.Invoke(configuration);

            builder.Services.AddSingleton(configuration);

            builder.Services.AddDbContext<OutboxEventContext>(optionsBuilder);

            builder.Services.AddTransient<Func<DbConnection, IOutboxEventService>>(
                _ => dbConnection =>
                    // this dbConnection will be passed on
                    // later on from implementations of integration service 
                {
                    var outboxEventContext = new OutboxEventContext(
                        new DbContextOptionsBuilder<OutboxEventContext>()
                            .UseSqlServer(dbConnection)
                            .Options);

                    return new OutboxEventService(outboxEventContext);
                });

            builder.Services.AddTransient<ITransactionalOutboxService, TransactionalOutboxService>();

            builder.Services.AddScoped<IOutboxMessageRelay, OutboxMessageRelay>();

            if (enableAutoMigration)
                builder.Services.AddHostedService<OutboxMigratorHostedService>();

            return builder; //.Services;
        }

        public static WrapperizerContext AddMessageRelayServices(
            this WrapperizerContext builder,
            Action<DbContextOptionsBuilder> optionsBuilder)
        {
            if (_outboxServicesEnabled)
                throw new InvalidOperationException(InvalidOperationExceptionMessage);

            builder.Services.AddDbContext<OutboxEventContext>(optionsBuilder, Singleton, Singleton);
            builder.Services.AddSingleton<IOutboxEventService, OutboxEventService>();
            builder.Services.AddSingleton<IOutboxMessageRelay, OutboxMessageRelay>();

            return builder; //.Services;
        }
    }
}
