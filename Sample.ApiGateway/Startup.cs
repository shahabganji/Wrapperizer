using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace Sample.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) 
            => services.AddOcelot(this.Configuration.GetSection("Ocelot")).AddConsul();

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
            => await app.UseOcelot();
    }
}
