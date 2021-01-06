using System;
using System.Collections.Generic;
using Funx.Extensions;

namespace Wrapperizer.Domain.Abstractions
{
    public class Result : IResult
    {
        public bool Success => _messages.SafeAny();

        private readonly List<string> _messages;
        public IReadOnlyList<string> Messages => _messages;

        private Result(IEnumerable<string> messages)
        {
            _messages = new List<string>(messages);
        }

        internal Result()
        {
        }

        public static Result Fail(params string[] messages) => new(messages);
        public static Result Fail(IEnumerable<string> messages) => new(messages);
        public static Result<T> Done<T>(T data) => data;
    }
    
    public class Result<T> : IConvertibleToResult
    {
        public bool Success => ParentResult.Success;

        public IReadOnlyList<string> Messages => ParentResult.Messages;

        public T Data { get; private set; }

        private Result ParentResult { get; set; }

        Result IConvertibleToResult.ParentResult => ParentResult;

        private Result(Result result)
        {
            ParentResult = result ?? throw new NullReferenceException(nameof(result));
        }

        private Result(Result result, T data)
            : this(result)
        {
            Data = data ?? throw new NullReferenceException(nameof(data));
        }

        public static implicit operator Result<T>(T data) => new(new Result(), data);
        public static implicit operator Result<T>(Result result) => new(result);
    }
}
