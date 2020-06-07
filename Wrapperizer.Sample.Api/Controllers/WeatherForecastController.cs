using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Abstraction;
using Wrapperizer.Sample.Api.Queries;

namespace Wrapperizer.Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ICommandQueryManager _commandQueryManager;

        public WeatherForecastController(
            ICommandQueryManager commandQueryManager)
        {
            _commandQueryManager = commandQueryManager;
        }

        [HttpGet]
        public async Task<object> Get()
            => await _commandQueryManager.Send(new GetWeatherForecast
            {
                DateTime = DateTime.Now.AddDays(1)
            });
    }
}
