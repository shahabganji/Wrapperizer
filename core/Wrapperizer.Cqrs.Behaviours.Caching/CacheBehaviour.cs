using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Wrapperizer.Cqrs.Abstractions.Commands;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Behaviours.Caching
{
    public sealed class CacheBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, ViewResult<TResponse>>
    {
        private readonly IDistributedCache _distributedCache;

        public CacheBehaviour(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache
                                ?? throw new ArgumentNullException(nameof(distributedCache),
                                    "Please configure distributed cache in DI container of the application.");
        }

        public async Task<ViewResult<TResponse>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<ViewResult<TResponse>> next)
        {
            if (next is null) throw new ArgumentNullException(nameof(next));

            if (request is ICommand<TResponse>)
                return await next();

            var key = JsonConvert.SerializeObject(request);

            var bytes = await _distributedCache.GetAsync(key, cancellationToken);

            return (bytes != null)
                ? JsonConvert.DeserializeObject<TResponse>(Encoding.UTF8.GetString(bytes))
                : await CallAndCache(key, next, cancellationToken).ConfigureAwait(false);
        }

        private async Task<ViewResult<TResponse>> CallAndCache(string key, RequestHandlerDelegate<ViewResult<TResponse>> next,
            CancellationToken cancellationToken)
        {
            var response = await next().ConfigureAwait(false);

            if (!response.Success) return response;

            var json = JsonConvert.SerializeObject(response);
            await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(json), cancellationToken)
                .ConfigureAwait(false);

            return response;
        }
    }
}
