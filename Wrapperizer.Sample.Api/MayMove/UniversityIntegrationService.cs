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
        public static IServiceCollection AddOutboxServices(this IServiceCollection services, SqlServerConnection connection)
        {
            services.AddDbContext<OutboxEventContext>(options =>
            {
                options.UseSqlServer(connection.ConnectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            services.AddTransient<Func<DbConnection, IOutboxEventService>>(
                sp => (DbConnection c) => new OutboxEventService(c));

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
        private readonly OutboxEventContext _outboxEventContext;
        private readonly ILogger<UniversityIntegrationService> _logger;
        private readonly IOutboxEventService _outboxEventService;

        public UniversityIntegrationService(
            ITransactionalUnitOfWork unitOfWork,
            OutboxEventContext outboxEventContext,
            Func<IDbConnection, IOutboxEventService> integrationServiceFactory,
            ILogger<UniversityIntegrationService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _outboxEventContext = outboxEventContext;
            _logger = logger;
            _outboxEventService = integrationServiceFactory(_unitOfWork.GetDbConnection());
        }

        public Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            throw new NotImplementedException();
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
