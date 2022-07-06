using System.Text;
using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using NUnit.Framework;

namespace Infra.Caching.Memory.IntegrationTest;

public class MemoryCacheTests
{
    private readonly ICache _cache;
    private readonly Encoding _encoding;

    public MemoryCacheTests()
    {
        var startup = new Startup();

        _cache = startup.GetService<ICache>();
        _encoding = Encoding.UTF8;
    }

    #region Sync

    [Test]
    public void SetSuccess() => _cache.Set("key", _encoding.GetBytes("value"));

    [Test]
    public void SetWithSlidingExpirationSuccess() =>
        _cache.Set("key", _encoding.GetBytes("value"), new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public void SetWithAbsoluteExpirationSuccess() =>
        _cache.Set("key", _encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public void SetWithAbsoluteExpirationRelativeToNowSuccess() =>
        _cache.Set("key", _encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public void SetStringSuccess() => _cache.SetString("key", "value");

    [Test]
    public void SetStringWithSlidingExpirationSuccess() =>
        _cache.SetString("key", "value", new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public void SetStringWithAbsoluteExpirationSuccess() =>
        _cache.SetString("key", "value", new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public void SetStringWithAbsoluteExpirationRelativeToNowSuccess() =>
        _cache.SetString("key", "value", new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public void GetSuccess()
    {
        const string key = "key";
        const string value = "value";

        _cache.Set(key, _encoding.GetBytes(value));

        var cachedValue = _cache.Get(key);

        Assert.Multiple(() =>
        {
            Assert.That(cachedValue, Is.Not.Empty);
            Assert.That(_encoding.GetString(cachedValue), Is.EqualTo(value));
        });
    }

    [Test]
    public void GetStringSuccess()
    {
        const string key = "key";
        const string value = "value";

        _cache.SetString(key, value);

        var cachedValue = _cache.GetString(key);

        Assert.Multiple(() =>
        {
            Assert.That(cachedValue, Is.Not.Empty);
            Assert.That(cachedValue, Is.EqualTo(value));
        });
    }

    [Test]
    public void RemoveSuccess()
    {
        const string key = "key";
        const string value = "value";

        _cache.Set(key, _encoding.GetBytes(value));

        _cache.Remove(key);

        Assert.That(_cache.Get(key), Is.Null);
    }

    [Test]
    public void RefreshNotSupported()
        => Assert.Throws<NotSupportedException>(() => _cache.Refresh("key"));

    #endregion

    #region Async

    [Test]
    public void SetAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => _cache.SetAsync("key", _encoding.GetBytes("value")));

    [Test]
    public void SetStringAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => _cache.SetStringAsync("key", "value"));

    [Test]
    public void GetAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => _cache.GetAsync("key"));

    [Test]
    public void GetStringAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => _cache.GetStringAsync("key"));

    [Test]
    public void RemoveAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => _cache.RemoveAsync("key"));

    [Test]
    public void RefreshAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => _cache.RefreshAsync("key"));

    #endregion
}
