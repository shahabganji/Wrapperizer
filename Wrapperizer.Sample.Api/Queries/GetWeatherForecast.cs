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
        public sealed class GetWeatherForecastHandler : IQueryHandler<GetWeatherForecast, IEnumerable<WeatherForecast>>
        {
            private readonly ICrudRepository<WeatherForecast> _repository;

            public GetWeatherForecastHandler(ICrudRepository<WeatherForecast> repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<WeatherForecast>> Handle(
                GetWeatherForecast request, CancellationToken cancellationToken)
                => await _repository.FindBy(_ => true);
        }
    }
}
