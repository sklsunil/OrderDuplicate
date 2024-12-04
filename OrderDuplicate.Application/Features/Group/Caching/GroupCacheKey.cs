using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace OrderDuplicate.Application.Features.Group.Caching
{
    public static class GroupCacheKey
    {
        private static readonly TimeSpan refreshInterval = TimeSpan.FromHours(1);
        public const string GetAllCacheKey = "all-groups";
        public static string GetPaginationCacheKey(string parameters)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(parameters);
            return $"GroupWithPaginationQuery-{Convert.ToBase64String(plainTextBytes)}";
        }
        static GroupCacheKey()
        {
            _tokensource = new CancellationTokenSource(refreshInterval);
        }
        private static CancellationTokenSource _tokensource;
        public static CancellationTokenSource SharedExpiryTokenSource()
        {
            if (_tokensource.IsCancellationRequested)
            {
                _tokensource = new CancellationTokenSource(refreshInterval);
            }
            return _tokensource;
        }
        public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
    }
}

