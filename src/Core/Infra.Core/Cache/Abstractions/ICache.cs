using System.Threading;
using System.Threading.Tasks;
using Infra.Core.Cache.Models;

#pragma warning disable CA1716

namespace Infra.Core.Cache.Abstractions;

public interface ICache
{
    #region Sync Method

    void Set(string key, byte[] value, CacheOptions cacheOptions = null);

    void SetString(string key, string value, CacheOptions cacheOptions = null);

    byte[] Get(string key);

    string GetString(string key);

    void Remove(string key);

    void Refresh(string key);

    #endregion

    #region Async Method

    Task SetAsync(string key, byte[] value, CacheOptions cacheOptions = null, CancellationToken token = default);

    Task SetStringAsync(string key, string value, CacheOptions cacheOptions = null, CancellationToken token = default);

    Task<byte[]> GetAsync(string key, CancellationToken token = default);

    Task<string> GetStringAsync(string key, CancellationToken token = default);

    Task RemoveAsync(string key, CancellationToken token = default);

    Task RefreshAsync(string key, CancellationToken token = default);

    #endregion
}
