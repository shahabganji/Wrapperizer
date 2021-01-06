using System.Collections.Generic;

namespace Wrapperizer.Domain.Abstractions
{
    public interface IResult
    {
        bool Success { get; }
        IReadOnlyList<string> Messages { get; }
    }

    internal interface IConvertibleToResult : IResult
    {
        internal Result ParentResult { get; }
    }
}
