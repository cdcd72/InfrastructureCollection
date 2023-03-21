# Infra.Caching.Memory

透過 Microsoft.Extensions.Caching.Memory 實現記憶體快取機制。  
Implement memory cache mechanism with Microsoft.Extensions.Caching.Memory.

## How to use

> 新增記憶體快取實例至 DI 容器中。

1. Add memory cache instance to DI container

    ```csharp
    builder.Services.AddMemoryCache();

    // MemoryCache namespace from Infra.Caching.Memory, not from Microsoft.Extensions.Caching.Memory!
    builder.Services.AddSingleton<ICache, MemoryCache>();
    ```

> 注入 ICache 來使用記憶體快取。

2. Inject ICache to use memory cache.
