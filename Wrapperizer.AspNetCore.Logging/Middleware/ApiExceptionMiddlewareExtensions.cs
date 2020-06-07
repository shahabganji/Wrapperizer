using System;
using Microsoft.AspNetCore.Builder;
using Wrapperizer.AspNetCore.Logging.Middleware;

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
            
            return builder.UseMiddleware<ApiExceptionMiddleware>(options);
        }
        public static IApplicationBuilder UseWrapperizerApiExceptionHandler(this IApplicationBuilder builder)
        {
            var options = new ApiExceptionOptions();            
            return builder.UseMiddleware<ApiExceptionMiddleware>(options);
        }
        
    }
}
