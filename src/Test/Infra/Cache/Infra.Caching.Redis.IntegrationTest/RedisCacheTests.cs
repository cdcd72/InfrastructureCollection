using System.Text;
using Infra.Core.Cache.Abstractions;
using Infra.Core.Cache.Models;
using NUnit.Framework;

namespace Infra.Caching.Redis.IntegrationTest;

public class RedisCacheTests
{
    private readonly ICache _cache;
    private readonly Encoding _encoding;

    public RedisCacheTests()
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
    public void RefreshSuccess()
    {
        const string key = "key";
        const string value = "value";

        _cache.Set(key, _encoding.GetBytes(value));
        _cache.Refresh(key);
    }

    #endregion

    #region Async

    [Test]
    public async Task SetSuccessAsync() => await _cache.SetAsync("key", _encoding.GetBytes("value"));

    [Test]
    public async Task SetWithSlidingExpirationSuccessAsync() =>
        await _cache.SetAsync("key", _encoding.GetBytes("value"), new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public async Task SetWithAbsoluteExpirationSuccessAsync() =>
        await _cache.SetAsync("key", _encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public async Task SetWithAbsoluteExpirationRelativeToNowSuccessAsync() =>
        await _cache.SetAsync("key", _encoding.GetBytes("value"), new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public async Task SetStringSuccessAsync() => await _cache.SetStringAsync("key", "value");

    [Test]
    public async Task SetStringWithSlidingExpirationSuccessAsync() =>
        await _cache.SetStringAsync("key", "value", new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

    [Test]
    public async Task SetStringWithAbsoluteExpirationSuccessAsync() =>
        await _cache.SetStringAsync("key", "value", new CacheOptions
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        });

    [Test]
    public async Task SetStringWithAbsoluteExpirationRelativeToNowSuccessAsync() =>
        await _cache.SetStringAsync("key", "value", new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });

    [Test]
    public async Task GetSuccessAsync()
    {
        const string key = "key";
        const string value = "value";

        await _cache.SetAsync(key, _encoding.GetBytes(value));

        var cachedValue = await _cache.GetAsync(key);

        Assert.Multiple(() =>
        {
            Assert.That(cachedValue, Is.Not.Empty);
            Assert.That(_encoding.GetString(cachedValue), Is.EqualTo(value));
        });
    }

    [Test]
    public async Task GetStringSuccessAsync()
    {
        const string key = "key";
        const string value = "value";

        await _cache.SetStringAsync(key, value);

        var cachedValue = await _cache.GetStringAsync(key);

        Assert.Multiple(() =>
        {
            Assert.That(cachedValue, Is.Not.Empty);
            Assert.That(cachedValue, Is.EqualTo(value));
        });
    }

    [Test]
    public async Task RemoveSuccessAsync()
    {
        const string key = "key";
        const string value = "value";

        await _cache.SetAsync(key, _encoding.GetBytes(value));

        await _cache.RemoveAsync(key);

        Assert.That(await _cache.GetAsync(key), Is.Null);
    }

    [Test]
    public async Task RefreshSuccessAsync()
    {
        const string key = "key";
        const string value = "value";

        await _cache.SetAsync(key, _encoding.GetBytes(value));
        await _cache.RefreshAsync(key);
    }

    #endregion
}
