using System;
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
        private static bool _outboxServicesEnabled;
        private static bool _messageRelayServicesEnabled;

        private static readonly string InvalidOperationExceptionMessage =
            $"You should enable either 'MessageRelay' services or 'Outbox' services; respectively using " +
            $"{nameof(AddMessageRelayServices)} or {nameof(AddOutboxServices)}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsBuilder">This should point to the same database as the one for UnitOfWork</param>
        /// <param name="configure"></param>
        /// <param name="enableAutoMigration"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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

            // ToDo: Change this to accept db from outside services not only the connection
            // and bear in mind that this one is a factory method
            builder.Services.AddTransient<Func<IOutboxEventService>>(
                services => () =>
                {
                    var outboxEventContext = services.GetRequiredService<OutboxEventContext>();
                    // new OutboxEventContext(
                    // new DbContextOptionsBuilder<OutboxEventContext>()
                    //     .UseSqlServer(dbConnection)
                    //     .Options);

                    return new OutboxEventService(outboxEventContext);
                });

            builder.Services.AddTransient<ITransactionalOutboxService, TransactionalOutboxService>();

            builder.Services.AddScoped<IOutboxMessageRelay, OutboxMessageRelay>();

            if (enableAutoMigration)
                builder.Services.AddHostedService<OutboxMigratorHostedService>();

            _outboxServicesEnabled = true;
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

            _messageRelayServicesEnabled = true;
            return builder; //.Services;
        }
    }
}
