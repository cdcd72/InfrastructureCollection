using Infra.Caching.Redis.Extensions;
using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Infra.Caching.Redis;

public class RedisCache : ICache
{
    private readonly IDistributedCache cache;

    public RedisCache(IDistributedCache cache) => this.cache = cache;

    #region Sync Method

    public void Set(string key, byte[] value, CacheOptions cacheOptions)
    {
        var distributedCacheEntryOptions =
            cacheOptions?.ToDistributedCacheEntryOptions() ?? new DistributedCacheEntryOptions();

        cache.Set(key, value, distributedCacheEntryOptions);
    }

    public void SetString(string key, string value, CacheOptions cacheOptions)
    {
        var distributedCacheEntryOptions =
            cacheOptions?.ToDistributedCacheEntryOptions() ?? new DistributedCacheEntryOptions();

        cache.SetString(key, value, distributedCacheEntryOptions);
    }

    public byte[] Get(string key) => cache.Get(key);

    public string GetString(string key) => cache.GetString(key);

    public void Remove(string key) => cache.Remove(key);

    public void Refresh(string key) => cache.Refresh(key);

    #endregion

    #region Async Method

    public async Task SetAsync(string key, byte[] value, CacheOptions cacheOptions, CancellationToken token = default)
    {
        var distributedCacheEntryOptions =
            cacheOptions?.ToDistributedCacheEntryOptions() ?? new DistributedCacheEntryOptions();

        await cache.SetAsync(key, value, distributedCacheEntryOptions, token);
    }

    public async Task SetStringAsync(string key, string value, CacheOptions cacheOptions, CancellationToken token = default)
    {
        var distributedCacheEntryOptions =
            cacheOptions?.ToDistributedCacheEntryOptions() ?? new DistributedCacheEntryOptions();

        await cache.SetStringAsync(key, value, distributedCacheEntryOptions, token);
    }

    public async Task<byte[]> GetAsync(string key, CancellationToken token = default) => await cache.GetAsync(key, token);

    public async Task<string> GetStringAsync(string key, CancellationToken token = default) => await cache.GetStringAsync(key, token);

    public async Task RemoveAsync(string key, CancellationToken token = default) => await cache.RemoveAsync(key, token);

    public async Task RefreshAsync(string key, CancellationToken token = default) => await cache.RefreshAsync(key, token);

    #endregion
}
