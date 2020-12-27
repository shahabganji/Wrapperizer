using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Extensions.Domain.AspNetCore
{
    public static class ViewResultExtensions
    {
        public static IActionResult AsActionResult(this IResult result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            var statusCode = result.Success
                ? StatusCodes.Status200OK
                : StatusCodes.Status500InternalServerError;

            return new OkObjectResult(result) {StatusCode = statusCode};
        }
    }
}
