using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sample.University.Notification.Extensions;
using Serilog;
using Wrapperizer;

namespace Sample.University.Notification
{
    public class Program
    {
        public const string AppName = "University Background Task";
        private  static IConfiguration Configuration { get; set; }
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((host, builder) =>
                {
                    Configuration = builder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true,
                            reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                        .Build();
                })
                .UseSerilog((context, configuration) =>
                {
                    configuration.WithWrapperizerConfiguration(Configuration, loggingConfiguration =>
                    {
                        loggingConfiguration.ApplicationName = AppName;
                    });
                });
        
        
        
    }
}
