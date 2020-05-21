using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Funx.Extensions;
using MediatR;

namespace Wrapperizer.Sample.Api
{
    public sealed class CacheBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private static readonly Dictionary<Type, TResponse> ResponseDictionary
            = new Dictionary<Type, TResponse>();

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
             return ResponseDictionary
                .Lookup(typeof(TRequest))
                .MatchAsync(
                    AddToCache(next),
                    item => Task.FromResult<TResponse>(item));
        }

        private static Func<Task<TResponse>> AddToCache(RequestHandlerDelegate<TResponse> next) =>
            async () =>
            {
                var response = await next();
                ResponseDictionary.TryAdd(typeof(TRequest) , response);
                return response;
            };
    }
}
