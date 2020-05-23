using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Funx.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Core.Abstraction.Specifications;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Repositories;
using Wrapperizer.Sample.Api.Queries;

namespace Wrapperizer.Sample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddEntityFrameworkInMemoryDatabase();
            
            services.AddWrapperizer().AddHandlers(configure:
                    collection =>
                    {
                        collection
                            .AddGlobalValidation()
                            .AddGlobalCaching();
                    })
                .AddCrudRepositories<WeatherForecastDbContext>((provider, options) =>
                {
                    options.UseInMemoryDatabase("WeatherForecast");
                    options.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                });

            services.AddTransient<Specification<GetWeatherForecast>, NotPastSpecification>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            SeedDatabase(app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ICrudRepository<WeatherForecast>>());

        }

        private static void SeedDatabase(ICrudRepository<WeatherForecast> repository)
        {
            var rng = new Random();
            new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            }.ForEach((summary,index) =>
            {
                repository.Add(new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = summary
                });
            });
        }
    }
}
