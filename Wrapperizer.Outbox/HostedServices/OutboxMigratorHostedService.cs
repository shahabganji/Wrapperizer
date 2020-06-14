using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Wrapperizer.Outbox.HostedServices
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class OutboxMigratorHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxMigratorHostedService> _logger;

        public OutboxMigratorHostedService(IServiceProvider serviceProvider,
            ILogger<OutboxMigratorHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start migrating database in background for Outbox context");

            using var scope = _serviceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<OutboxEventContext>();

            try
            {
                await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Migration done for Outbox context");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Database migration failed for Outbox context");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
