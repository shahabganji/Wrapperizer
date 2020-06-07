using System;

namespace Wrapperizer.Extensions.Repositories.EfCore.Extensions
{
    internal static class ExceptionExtensions
    {
        public static string GetInnerMostExceptionMessage(this Exception ex)
            => ex.InnerException == null 
                ? ex.Message 
                : ex.InnerException.GetInnerMostExceptionMessage();
    }
}
