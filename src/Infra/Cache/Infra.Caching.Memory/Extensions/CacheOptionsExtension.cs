using Infra.Core.Cache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infra.Caching.Memory.Extensions;

public static class CacheOptionsExtension
{
    public static MemoryCacheEntryOptions ToMemoryCacheEntryOptions(this CacheOptions cacheOptions)
    {
        var memoryCacheEntryOptions = new MemoryCacheEntryOptions();

        if (cacheOptions.SlidingExpiration is not null)
            memoryCacheEntryOptions.SetSlidingExpiration(cacheOptions.SlidingExpiration.Value);

        if (cacheOptions.AbsoluteExpiration is not null)
            memoryCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpiration.Value);

        if (cacheOptions.AbsoluteExpirationRelativeToNow is not null)
            memoryCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpirationRelativeToNow.Value);

        return memoryCacheEntryOptions;
    }
}
