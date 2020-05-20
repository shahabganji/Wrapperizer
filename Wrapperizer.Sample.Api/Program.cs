using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Repositories;

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
                .ConfigureServices(configureDelegate: ((context, collection) =>
                {
                    collection.AddWrapperizer()
                        .AddHandlers(ServiceLifetime.Scoped)
                        .AddCrudRepositories()
                        .AddTransactionalUnitOfWork<DomainEventAwareDbContext>();
                }))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
