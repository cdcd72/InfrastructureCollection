using System.Threading.Tasks;
using Infra.Core.Cache.Models;

#pragma warning disable CA1716

namespace Infra.Core.Cache.Abstractions;

public interface ICache
{
    #region Sync Method

    void Set(string key, byte[] value, CacheOptions cacheOptions = null);

    byte[] Get(string key);

    void Remove(string key);

    void Refresh(string key);

    #endregion

    #region Async Method

    Task SetAsync(string key, byte[] value, CacheOptions cacheOptions = null);

    Task<byte[]> GetAsync(string key);

    Task RemoveAsync(string key);

    Task RefreshAsync(string key);

    #endregion
}
