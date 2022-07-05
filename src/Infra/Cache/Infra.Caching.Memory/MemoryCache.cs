using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infra.Caching.Memory;

public class MemoryCache : ICache
{
    private readonly IMemoryCache _cache;

    public MemoryCache(IMemoryCache cache) => _cache = cache;

    #region Sync Method

    public void Set(string key, byte[] value, CacheOptions? cacheOptions = null)
    {
        var memoryCacheEntryOptions = new MemoryCacheEntryOptions();

        if (cacheOptions?.SlidingExpiration is not null)
            memoryCacheEntryOptions.SetSlidingExpiration(cacheOptions.SlidingExpiration.Value);

        if (cacheOptions?.AbsoluteExpiration is not null)
            memoryCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpiration.Value);

        if (cacheOptions?.AbsoluteExpirationRelativeToNow is not null)
            memoryCacheEntryOptions.SetAbsoluteExpiration(cacheOptions.AbsoluteExpirationRelativeToNow.Value);

        _cache.Set(key, value, memoryCacheEntryOptions);
    }

    public byte[] Get(string key) => _cache.Get<byte[]>(key);

    public void Remove(string key) => _cache.Remove(key);

    public void Refresh(string key) => throw new NotSupportedException();

    #endregion

    #region Async Method

    public Task SetAsync(string key, byte[] value, CacheOptions? cacheOptions = null) => throw new NotSupportedException();

    public Task<byte[]> GetAsync(string key) => throw new NotSupportedException();

    public Task RemoveAsync(string key) => throw new NotSupportedException();

    public Task RefreshAsync(string key) => throw new NotSupportedException();

    #endregion
}
