using OrderDuplicate.Application.Common.Interfaces.Caching;

using LazyCache;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Common.Behaviours;

public class CacheInvalidationBehaviour<TRequest, TResponse>(
    IAppCache cache,
    ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger
        ) : IPipelineBehavior<TRequest, TResponse>
      where TRequest : ICacheInvalidatorRequest<TResponse>
{
    private readonly IAppCache _cache = cache;
    private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogTrace("{Name} cache expire with {@Request}.", nameof(request), request);
        var response = await next();
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            _cache.Remove(request.CacheKey);
        }
        request.SharedExpiryTokenSource?.Cancel();
        return response;
    }
}
