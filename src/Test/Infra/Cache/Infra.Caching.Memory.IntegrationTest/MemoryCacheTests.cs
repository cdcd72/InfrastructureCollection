using System.Text;
using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using NUnit.Framework;

namespace Infra.Caching.Memory.IntegrationTest;

public class MemoryCacheTests
{
    private readonly ICache cache;
    private readonly Encoding encoding;

    public MemoryCacheTests()
    {
        var startup = new Startup();

        cache = startup.GetService<ICache>();
        encoding = Encoding.UTF8;
    }

    #region Sync

    [Test]
    public void SetSuccess() => cache.Set("key", encoding.GetBytes("value"));

    [Test]
    public void SetWithSlidingExpirationSuccess() =>
        cache.Set("key", encoding.GetBytes("value"), new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public void SetWithAbsoluteExpirationSuccess() =>
        cache.Set("key", encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public void SetWithAbsoluteExpirationRelativeToNowSuccess() =>
        cache.Set("key", encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public void SetStringSuccess() => cache.SetString("key", "value");

    [Test]
    public void SetStringWithSlidingExpirationSuccess() =>
        cache.SetString("key", "value", new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public void SetStringWithAbsoluteExpirationSuccess() =>
        cache.SetString("key", "value", new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public void SetStringWithAbsoluteExpirationRelativeToNowSuccess() =>
        cache.SetString("key", "value", new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public void GetSuccess()
    {
        const string key = "key";
        const string value = "value";

        cache.Set(key, encoding.GetBytes(value));

        var cachedValue = cache.Get(key);

        Assert.Multiple(() =>
        {
            Assert.That(cachedValue, Is.Not.Empty);
            Assert.That(encoding.GetString(cachedValue), Is.EqualTo(value));
        });
    }

    [Test]
    public void GetStringSuccess()
    {
        const string key = "key";
        const string value = "value";

        cache.SetString(key, value);

        var cachedValue = cache.GetString(key);

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

        cache.Set(key, encoding.GetBytes(value));

        cache.Remove(key);

        Assert.That(cache.Get(key), Is.Null);
    }

    [Test]
    public void RefreshNotSupported()
        => Assert.Throws<NotSupportedException>(() => cache.Refresh("key"));

    #endregion

    #region Async

    [Test]
    public void SetAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => cache.SetAsync("key", encoding.GetBytes("value")));

    [Test]
    public void SetStringAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => cache.SetStringAsync("key", "value"));

    [Test]
    public void GetAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => cache.GetAsync("key"));

    [Test]
    public void GetStringAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => cache.GetStringAsync("key"));

    [Test]
    public void RemoveAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => cache.RemoveAsync("key"));

    [Test]
    public void RefreshAsyncNotSupported()
        => Assert.ThrowsAsync<NotSupportedException>(() => cache.RefreshAsync("key"));

    #endregion
}
