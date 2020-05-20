using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Core.Abstraction;

namespace Wrapperizer.Sample.Api.Queries
{
    public sealed class GetWeatherForecast : IQuery<IEnumerable<WeatherForecast>>
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        public sealed class GetWeatherForecastHandler : IQueryHandler<GetWeatherForecast,IEnumerable<WeatherForecast>>
        {
            public Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecast request, CancellationToken cancellationToken)
            {
                var rng = new Random();
                return Task.FromResult( 
                    Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    }));
                
            }
        }
    }
}
