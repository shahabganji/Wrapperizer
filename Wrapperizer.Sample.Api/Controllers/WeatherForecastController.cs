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
        private readonly IActionResultAdapter _resultAdapter;

        public WeatherForecastController(
            ICommandQueryManager commandQueryManager,
            IActionResultAdapter resultAdapter
            )
        {
            _commandQueryManager = commandQueryManager;
            _resultAdapter = resultAdapter;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _commandQueryManager.Send(new GetWeatherForecast
             {
                 DateTime = DateTime.Now.AddDays(1)
             });
             return _resultAdapter.Result;
        }
    }
}
