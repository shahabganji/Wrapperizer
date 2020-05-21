using Microsoft.EntityFrameworkCore;
using Wrapperizer.Repositories;

namespace Wrapperizer.Sample.Api
{
    public sealed class WeatherForecastDbContext : DbContext
    {
        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    }
}
