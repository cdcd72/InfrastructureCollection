# Infra.Time

實現時間包裝器。  
Implement time wrapper.

## How to use

> 新增時間包裝器實例至 DI 容器中。

1. Add time wrapper instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddSingleton<ITimeWrapper, TimeWrapper>();
    }
    ```

> 注入 ITimeWrapper 來使用時間包裝器。

2. Inject ITimeWrapper to use time wrapper.
