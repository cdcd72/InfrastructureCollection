using System.Text;
using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using NUnit.Framework;

namespace Infra.Caching.Redis.IntegrationTest;

public class RedisCacheTests
{
    private readonly ICache cache;
    private readonly Encoding encoding;

    public RedisCacheTests()
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
    public void RefreshSuccess()
    {
        const string key = "key";
        const string value = "value";

        cache.Set(key, encoding.GetBytes(value));
        cache.Refresh(key);
    }

    #endregion

    #region Async

    [Test]
    public async Task SetAsyncSuccess() => await cache.SetAsync("key", encoding.GetBytes("value"));

    [Test]
    public async Task SetAsyncWithSlidingExpirationSuccess() =>
        await cache.SetAsync("key", encoding.GetBytes("value"), new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public async Task SetAsyncWithAbsoluteExpirationSuccess() =>
        await cache.SetAsync("key", encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public async Task SetAsyncWithAbsoluteExpirationRelativeToNowSuccess() =>
        await cache.SetAsync("key", encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public async Task SetStringAsyncSuccess() => await cache.SetStringAsync("key", "value");

    [Test]
    public async Task SetStringAsyncWithSlidingExpirationSuccess() =>
        await cache.SetStringAsync("key", "value", new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public async Task SetStringAsyncWithAbsoluteExpirationSuccess() =>
        await cache.SetStringAsync("key", "value", new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public async Task SetStringAsyncWithAbsoluteExpirationRelativeToNowSuccess() =>
        await cache.SetStringAsync("key", "value", new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public async Task GetAsyncSuccess()
    {
        const string key = "key";
        const string value = "value";

        await cache.SetAsync(key, encoding.GetBytes(value));

        var cachedValue = await cache.GetAsync(key);

        Assert.Multiple(() =>
        {
            Assert.That(cachedValue, Is.Not.Empty);
            Assert.That(encoding.GetString(cachedValue), Is.EqualTo(value));
        });
    }

    [Test]
    public async Task GetStringAsyncSuccess()
    {
        const string key = "key";
        const string value = "value";

        await cache.SetStringAsync(key, value);

        var cachedValue = await cache.GetStringAsync(key);

        Assert.Multiple(() =>
        {
            Assert.That(cachedValue, Is.Not.Empty);
            Assert.That(cachedValue, Is.EqualTo(value));
        });
    }

    [Test]
    public async Task RemoveAsyncSuccess()
    {
        const string key = "key";
        const string value = "value";

        await cache.SetAsync(key, encoding.GetBytes(value));

        await cache.RemoveAsync(key);

        Assert.That(await cache.GetAsync(key), Is.Null);
    }

    [Test]
    public async Task RefreshAsyncSuccess()
    {
        const string key = "key";
        const string value = "value";

        await cache.SetAsync(key, encoding.GetBytes(value));
        await cache.RefreshAsync(key);
    }

    #endregion
}
