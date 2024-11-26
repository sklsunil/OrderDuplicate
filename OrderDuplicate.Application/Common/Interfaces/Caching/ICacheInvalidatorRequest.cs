using MediatR;

namespace OrderDuplicate.Application.Common.Interfaces.Caching;

public interface ICacheInvalidatorRequest<TResponse> : IRequest<TResponse>
{
    string CacheKey { get => string.Empty; }
    CancellationTokenSource? SharedExpiryTokenSource { get; }
}
