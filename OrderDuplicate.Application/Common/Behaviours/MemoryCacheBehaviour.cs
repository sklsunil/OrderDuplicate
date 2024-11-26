using OrderDuplicate.Application.Common.Interfaces.Caching;

using LazyCache;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Common.Behaviours;

public class MemoryCacheBehaviour<TRequest, TResponse>(
    IAppCache cache,
    ILogger<MemoryCacheBehaviour<TRequest, TResponse>> logger
        ) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    private readonly IAppCache _cache = cache;
    private readonly ILogger<MemoryCacheBehaviour<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogTrace("{Name} is caching with {@Request}.", nameof(request), request);
        var response = await _cache.GetOrAddAsync(
            request.CacheKey,
            async () => await next(),
            request.Options);

        return response;
    }
}
