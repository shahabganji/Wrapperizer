using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Core.Abstraction.Specifications;

namespace Wrapperizer.Sample.Api.Queries
{

    public class NotPastSpecification : Specification<GetWeatherForecast>
    {
        public override Expression<Func<GetWeatherForecast, bool>> ToExpression()
        {
            return g => g.DateTime >= DateTime.Now;
        }
    }
    public sealed class GetWeatherForecast : IQuery<IEnumerable<WeatherForecast>>
    {
        
        
        public DateTime DateTime { get; set; }
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
