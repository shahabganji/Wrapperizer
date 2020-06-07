using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wrapperizer.Abstraction;

namespace Wrapperizer.Sample.Api
{
    public class WeatherForecast : Entity<Guid> , IAggregateRoot
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

        public string Summary { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null!;
        }
    }
}
