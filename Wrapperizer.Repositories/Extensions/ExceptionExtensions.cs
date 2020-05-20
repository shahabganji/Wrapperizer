using System;

namespace Fotokar.Infrastructure.Persistence.Extensions
{
    internal static class ExceptionExtensions
    {
        public static string GetInnerMostExceptionMessage(this Exception ex)
            => ex.InnerException == null 
                ? ex.Message 
                : ex.InnerException.GetInnerMostExceptionMessage();
    }
}
