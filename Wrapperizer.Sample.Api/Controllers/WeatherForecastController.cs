using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Sample.Api.Queries;

namespace Wrapperizer.Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ICommandQueryManager _commandQueryManager;
        public WeatherForecastController(ICommandQueryManager commandQueryManager) 
            => _commandQueryManager = commandQueryManager;

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
            => await _commandQueryManager.Send(new GetWeatherForecast());
    }
}
