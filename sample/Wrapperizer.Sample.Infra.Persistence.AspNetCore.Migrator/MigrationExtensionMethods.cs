using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Sample.Infra.Persistence.AspNetCore.Migrator.HostedService;

namespace Wrapperizer.Sample.Infra.Persistence.AspNetCore.Migrator
{
    public static class MigrationExtensionMethods
    {
        public static IServiceCollection AddUniversityMigrator(this IServiceCollection services)
         => services.AddHostedService<MigratorHostedService>();

    }
}
