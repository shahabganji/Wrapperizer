using System;
using Microsoft.Extensions.Logging;

namespace Wrapperizer.Extensions.AspNetCore.Logging
{
    public static class LogMessages
    {
        private static readonly Action<ILogger, string, string, long, Exception> RoutePerformance;

        static LogMessages()
        {
            RoutePerformance = LoggerMessage.Define<string, string, long>(LogLevel.Information, 0,
                "{RouteName} {Method} code took {ElapsedInMilliseconds}.");            
        }

        public static void LogRoutePerformance(this ILogger logger, string pageName, string method, 
            long elapsedInMilliseconds)
        {
            RoutePerformance(logger, pageName, method, elapsedInMilliseconds, null);
        }
    }
}
