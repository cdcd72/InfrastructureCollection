using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Infra.Caching.Redis;

public class RedisCache : ICache
{
    private readonly IDistributedCache _cache;

    public RedisCache(IDistributedCache cache) => _cache = cache;

    #region Sync Method

    public void Set(string key, byte[] value, CacheOptions? cacheOptions = null)
    {
        var distributedCacheEntryOptions = new DistributedCacheEntryOptions();

        if (cacheOptions?.SlidingExpiration is not null)
            distributedCacheEntryOptions.SetSlidingExpiration(cacheOptions.SlidingExpiration.Value);

        if (cacheOptions?.AbsoluteExpiration is not null)
            distributedCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpiration.Value);

        if (cacheOptions?.AbsoluteExpirationRelativeToNow is not null)
            distributedCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpirationRelativeToNow.Value);

        _cache.Set(key, value, distributedCacheEntryOptions);
    }

    public byte[] Get(string key) => _cache.Get(key);

    public void Remove(string key) => _cache.Remove(key);

    public void Refresh(string key) => _cache.Refresh(key);

    #endregion

    #region Async Method

    public async Task SetAsync(string key, byte[] value, CacheOptions? cacheOptions = null)
    {
        var distributedCacheEntryOptions = new DistributedCacheEntryOptions();

        if (cacheOptions?.SlidingExpiration is not null)
            distributedCacheEntryOptions.SetSlidingExpiration(cacheOptions.SlidingExpiration.Value);

        if (cacheOptions?.AbsoluteExpiration is not null)
            distributedCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpiration.Value);

        if (cacheOptions?.AbsoluteExpirationRelativeToNow is not null)
            distributedCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpirationRelativeToNow.Value);

        await _cache.SetAsync(key, value, distributedCacheEntryOptions);
    }

    public async Task<byte[]> GetAsync(string key) => await _cache.GetAsync(key);

    public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);

    public async Task RefreshAsync(string key) => await _cache.RefreshAsync(key);

    #endregion
}
