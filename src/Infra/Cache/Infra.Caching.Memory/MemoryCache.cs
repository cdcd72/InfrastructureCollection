using Infra.Caching.Memory.Extensions;
using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infra.Caching.Memory;

public class MemoryCache(IMemoryCache cache) : ICache
{
    #region Sync Method

    public void Set(string key, byte[] value, CacheOptions cacheOptions)
        => cache.Set(key, value, cacheOptions?.ToMemoryCacheEntryOptions());

    public void SetString(string key, string value, CacheOptions cacheOptions)
        => cache.Set(key, value, cacheOptions?.ToMemoryCacheEntryOptions());

    public byte[] Get(string key) => cache.Get<byte[]>(key);

    public string GetString(string key) => cache.Get<string>(key);

    public void Remove(string key) => cache.Remove(key);

    public void Refresh(string key) => throw new NotSupportedException();

    #endregion

    #region Async Method

    public Task SetAsync(string key, byte[] value, CacheOptions cacheOptions, CancellationToken token = default) => throw new NotSupportedException();

    public Task SetStringAsync(string key, string value, CacheOptions cacheOptions, CancellationToken token = default) => throw new NotSupportedException();

    public Task<byte[]> GetAsync(string key, CancellationToken token = default) => throw new NotSupportedException();

    public Task<string> GetStringAsync(string key, CancellationToken token = default) => throw new NotSupportedException();

    public Task RemoveAsync(string key, CancellationToken token = default) => throw new NotSupportedException();

    public Task RefreshAsync(string key, CancellationToken token = default) => throw new NotSupportedException();

    #endregion
}
