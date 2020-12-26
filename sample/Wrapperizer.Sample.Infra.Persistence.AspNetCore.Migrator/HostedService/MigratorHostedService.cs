using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Wrapperizer.Sample.Infra.Persistence.AspNetCore.Migrator.HostedService
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class MigratorHostedService: IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigratorHostedService> _logger;

        public MigratorHostedService(IServiceProvider serviceProvider, ILogger<MigratorHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start migrating database in background");
            
            using var scope = _serviceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();

            try
            {
                await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Migration done");
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, "Database migration failed");
            }

        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
