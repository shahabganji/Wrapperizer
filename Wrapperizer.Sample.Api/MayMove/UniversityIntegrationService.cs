using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Extensions.Repositories.EfCore;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;
using Wrapperizer.Outbox;
using Wrapperizer.Outbox.Services;
using Wrapperizer.Sample.Configurations;

namespace Wrapperizer.Sample.Api.MayMove
{
    public static class OutboxExtensions
    {
        public static IServiceCollection AddOutboxServices(this IServiceCollection services,
            SqlServerConnection connection)
        {
            services.AddDbContext<OutboxEventContext>(options =>
            {
                options.UseSqlServer(connection.ConnectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

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

            services.AddTransient<IUniversityIntegrationService, UniversityIntegrationService>();

            return services;
        }
    }

    public interface IUniversityIntegrationService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent @event);
    }

    public sealed class UniversityIntegrationService : IUniversityIntegrationService
    {
        private readonly ITransactionalUnitOfWork _unitOfWork;

        private readonly ILogger<UniversityIntegrationService> _logger;
        private readonly IOutboxEventService _outboxEventService;

        public UniversityIntegrationService(
            ITransactionalUnitOfWork unitOfWork,
            Func<DbConnection, IOutboxEventService> integrationServiceFactory,
            ILogger<UniversityIntegrationService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _outboxEventService = integrationServiceFactory(unitOfWork.GetDbConnection());
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _outboxEventService.RetrievePendingEventsToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                // _logger.LogInformation(
                //     "----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})",
                //     logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {
                    await _outboxEventService.MarkEventAsInProgressAsync(logEvt.EventId);
                    // _eventBus.Publish(logEvt.IntegrationEvent);
                    await _outboxEventService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}",
                    //     logEvt.EventId, Program.AppName);

                    await _outboxEventService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent @event)
        {
            _logger.LogInformation(
                "----- Enqueuing integration event {IntegrationEventId} to Outbox ({@IntegrationEvent})", @event.Id,
                @event);

            await _outboxEventService.SaveEventAsync(@event, _unitOfWork.GetCurrentTransaction());
        }
    }
}
