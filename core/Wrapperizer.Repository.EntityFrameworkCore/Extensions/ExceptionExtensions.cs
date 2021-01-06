using System;

namespace Wrapperizer.Repository.EntityFrameworkCore.Extensions
{
    internal static class ExceptionExtensions
    {
        public static string GetInnerMostExceptionMessage(this Exception ex)
            => ex.InnerException == null 
                ? ex.Message 
                : ex.InnerException.GetInnerMostExceptionMessage();
    }
}
