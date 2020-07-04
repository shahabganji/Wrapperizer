using System;
using System.Configuration;
using System.IO;
using Grace.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wrapperizer;
using Wrapperizer.AspNetCore.Logging.Configuration;
using Wrapperizer.Sample.Configurations;

namespace Sample.Api
{
    public class Program
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, builder) =>
                {
                    Configuration = builder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true,
                            reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .UseSerilog((context, configuration) =>
                {
                    configuration.WithWrapperizerConfiguration(Configuration,
                        loggingConfiguration =>
                        {
                            loggingConfiguration.ApplicationName = "Sample Api";
                            var sqlConnection = new SqlServerConnection();
                            Configuration.Bind("Infra:Connections:Sql", sqlConnection);
                            loggingConfiguration.SqlConnectionString = sqlConnection.ConnectionString;
                        });
                    // Serilog.Debugging.SelfLog.Enable(Console.Out);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseGrace();
    }
}
