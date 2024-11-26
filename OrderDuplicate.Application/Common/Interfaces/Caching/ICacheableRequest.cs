using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace OrderDuplicate.Application.Common.Interfaces.Caching;

public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get => string.Empty; }
    MemoryCacheEntryOptions? Options { get; }
}
