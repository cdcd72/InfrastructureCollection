# Infra.Caching.Redis

透過 Microsoft.Extensions.Caching.StackExchangeRedis 實現分散式快取機制。  
Implement distributed cache mechanism with Microsoft.Extensions.Caching.StackExchangeRedis.

## How to use

> 新增 Redis 快取實例至 DI 容器中。

1. Add redis cache instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddStackExchangeRedisCache(options =>
        {
            // The Redis instance name.
            options.InstanceName = Configuration.GetValue<string>("Redis:InstanceName");
            // The configuration used to connect to Redis.
            options.Configuration = Configuration.GetValue<string>("Redis:Configuration");
        });

        services.AddSingleton<ICache, RedisCache>();
    }
    ```

> 注入 ICache 來使用分散式快取。

2. Inject ICache to use distributed cache.
