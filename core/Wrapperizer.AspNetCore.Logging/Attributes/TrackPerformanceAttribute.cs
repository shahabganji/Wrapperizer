using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wrapperizer.AspNetCore.Logging.Attributes
{
    public class TrackPerformanceAttribute : ActionFilterAttribute , IFilterFactory
    {
        private readonly ILogger<TrackPerformanceAttribute> _logger;
        private readonly Stopwatch _timer;

        public TrackPerformanceAttribute()
        {
        }
        
        public TrackPerformanceAttribute(ILogger<TrackPerformanceAttribute> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {            
            _timer.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _timer.Stop();
            if (context.Exception == null)
            {
                _logger.LogRoutePerformance(context.HttpContext.Request.Path,
                    context.HttpContext.Request.Method,
                    _timer.ElapsedMilliseconds);
            }
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TrackPerformanceAttribute>>();
            return new TrackPerformanceAttribute(logger);
        }

        public bool IsReusable { get; }
    }
}
