using System;

namespace Wrapperizer.Sample.Domain.Exceptions
{
    public class InvalidNationalCodeException : Exception
    {
        public InvalidNationalCodeException(string message): base(message)
        {}
    }
}
