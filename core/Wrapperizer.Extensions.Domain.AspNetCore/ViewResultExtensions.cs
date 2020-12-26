using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Extensions.Domain.AspNetCore
{
    public static class ViewResultExtensions
    {
        public static IActionResult AsActionResult(this IViewResult viewResult)
        {
            if (viewResult is null)
                throw new ArgumentNullException(nameof(viewResult));

            var statusCode = viewResult.Success
                ? StatusCodes.Status200OK
                : StatusCodes.Status500InternalServerError;

            return new OkObjectResult(viewResult) {StatusCode = statusCode};
        }
    }
}
