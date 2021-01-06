using System;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Wrapperizer.Extensions.AspNetCore.Logging.Middleware;

// ReSharper disable once CheckNamespace
namespace Wrapperizer
{
    public static class ApiExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseWrapperizerApiExceptionHandler(this IApplicationBuilder builder, 
            Action<ApiExceptionOptions> configureOptions)
        {
            var options = new ApiExceptionOptions();
            configureOptions(options);

            builder.UseSerilogRequestLogging();
            return builder.UseMiddleware<ApiExceptionMiddleware>(options);
        }
        public static IApplicationBuilder UseWrapperizerApiExceptionHandler(this IApplicationBuilder builder)
        {
            var options = new ApiExceptionOptions();
            builder.UseSerilogRequestLogging();
            return builder.UseMiddleware<ApiExceptionMiddleware>(options);
        }
        
    }
}
