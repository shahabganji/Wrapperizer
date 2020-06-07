using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Funx.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Core.Abstraction;

namespace Wrapperizer.Core
{
    public sealed class CacheBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TResponse : class
    {
        private static readonly Dictionary<Type, object> ResponseDictionary
            = new Dictionary<Type, object>();

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var result = await ResponseDictionary
                .Lookup(typeof(TRequest))
                .MatchAsync(
                    AddToCacheAsync(next),
                    cachedValue => cachedValue as TResponse);

            return result;
        }

        private Func<Task<TResponse>> AddToCacheAsync(RequestHandlerDelegate<TResponse> next)
            => async () =>
            {
                var response = await next();
                ResponseDictionary.TryAdd(typeof(TRequest), response);
                return response;
            };
    }
}
