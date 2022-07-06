# Infra.Caching.Redis

透過 Microsoft.Extensions.Caching.StackExchangeRedis 實現分散式快取機制。  
Implement distributed cache mechanism with Microsoft.Extensions.Caching.StackExchangeRedis.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "Cache": {
        "Redis": {
            "InstanceName": "SampleInstance",
            "Configuration": "localhost:6379"
        }
    }
    ```

    - InstanceName：The Redis instance name
    - Configuration：The [configuration](https://stackexchange.github.io/StackExchange.Redis/Configuration.html) used to connect to Redis

> 新增 Redis 快取實例至 DI 容器中。

2. Add redis cache instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = Configuration.GetValue<string>("Cache:Redis:InstanceName");
            options.Configuration = Configuration.GetValue<string>("Cache:Redis:Configuration");
        });

        services.AddSingleton<ICache, RedisCache>();
    }
    ```

> 注入 ICache 來使用分散式快取。

3. Inject ICache to use distributed cache.
