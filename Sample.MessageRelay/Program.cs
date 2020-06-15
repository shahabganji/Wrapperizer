using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sample.MessageRelay.Extensions;
using Serilog;

namespace Sample.MessageRelay
{
    public class Program
    {
        public const string AppName = "Sample Message Relay";
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://localhost:8002");
                })
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: true);
                    builder.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true);
                    builder.AddEnvironmentVariables();
                    builder.AddCommandLine(args);
                }).ConfigureLogging((host, builder) =>
                {
                    builder.UseSerilog(host.Configuration);
                })
                .UseSerilog();
    }
}
