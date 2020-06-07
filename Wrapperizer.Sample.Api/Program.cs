using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Wrapperizer.Sample.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // .ConfigureServices(configureDelegate: ((context, collection) =>
                // {
                //     collection.AddWrapperizer()
                //         .AddHandlers(ServiceLifetime.Scoped)
                //         .AddCrudRepositories()
                //         .AddTransactionalUnitOfWork<DomainEventAwareDbContext>();
                // }))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
