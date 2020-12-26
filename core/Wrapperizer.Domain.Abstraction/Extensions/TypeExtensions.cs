using System;

namespace Wrapperizer.Domain.Abstraction.Extensions
{
    internal static class TypeExtensions
    {
        public static Type GetRealType<T>(this T t)
        {
            if( t is null ) throw new ArgumentNullException(nameof(t));

            var type = t.GetType();
            return type.ToString().Contains("Castle.Proxies.") ? type.BaseType : type;
        }
    }
}
