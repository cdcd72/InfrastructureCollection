using Infra.Core.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Infra.Caching.Redis.Extensions;

public static class CacheOptionsExtension
{
    public static DistributedCacheEntryOptions ToDistributedCacheEntryOptions(this CacheOptions cacheOptions)
    {
        var distributedCacheEntryOptions = new DistributedCacheEntryOptions();

        if (cacheOptions.SlidingExpiration is not null)
            distributedCacheEntryOptions.SetSlidingExpiration(cacheOptions.SlidingExpiration.Value);

        if (cacheOptions.AbsoluteExpiration is not null)
            distributedCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpiration.Value);

        if (cacheOptions.AbsoluteExpirationRelativeToNow is not null)
            distributedCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpirationRelativeToNow.Value);

        return distributedCacheEntryOptions;
    }
}
