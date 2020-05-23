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
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IActionResultAdapter _resultAdapter;

        private static readonly Dictionary<Type, IActionResult> ResponseDictionary
            = new Dictionary<Type, IActionResult>();

        public CacheBehaviour(IActionResultAdapter resultAdapter)
        {
            _resultAdapter = resultAdapter;
        }
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var result = await ResponseDictionary
                .Lookup(typeof(TRequest))
                .MatchAsync(
                    AddToCache(next),
                    cachedValue => Task.FromResult<IActionResult>(new OkObjectResult(cachedValue)));

            _resultAdapter.Result = result;
            return default;
            // todo: now what to do with the global adapter????
        }

        private Func<Task<IActionResult>> AddToCache(RequestHandlerDelegate<TResponse> next) =>
            async () =>
            {
                var response = await next();
                ResponseDictionary.TryAdd(typeof(TRequest) , _resultAdapter.Result);
                return _resultAdapter.Result;
            };
    }
}
