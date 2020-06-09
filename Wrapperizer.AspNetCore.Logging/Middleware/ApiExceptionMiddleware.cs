using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wrapperizer.Extensions.Cqrs.Exceptions;

namespace Wrapperizer.AspNetCore.Logging.Middleware
{
    public class ApiExceptionMiddleware
    {
       private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;
        private readonly ApiExceptionOptions _options;

        public ApiExceptionMiddleware(ApiExceptionOptions options, RequestDelegate next, 
            ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidRequestException requestException)
            {
                await HandleInvalidRequestExceptionAsync(context, requestException);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleInvalidRequestExceptionAsync(HttpContext context, InvalidRequestException exception)
        {
            var result = JsonSerializer.Serialize(exception.ValidationResults , new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync(result);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            
            var error = new ApiError
            {
                TraceIdentifier = context.TraceIdentifier,
                
                Id = Guid.NewGuid().ToString(),
                Status = (short)HttpStatusCode.InternalServerError,
                Title = "Some kind of error occurred in the API.  Please use the id and contact our " +
                        "support team if the problem persists."
            };
            
            _options.AddResponseDetails?.Invoke(context, exception, error);            

            var innerExMessage = GetInnermostExceptionMessage(exception);

            var level = _options.DetermineLogLevel?.Invoke(exception) ?? LogLevel.Error;
            _logger.Log(level, exception, "BADNESS!!! " + innerExMessage  + " -- {ErrorId}.", error.Id);           

            var result = JsonSerializer.Serialize(error , new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }

        private string GetInnermostExceptionMessage(Exception exception)
        {
            if (exception.InnerException != null)
                return GetInnermostExceptionMessage(exception.InnerException);

            return exception.Message;
        }
    }
}
