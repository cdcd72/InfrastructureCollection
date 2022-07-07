# Infra.Hash

實現雜湊機制。  
Implement hash mechanism.

## How to use

> 新增雜湊工廠實例至 DI 容器中。

1. Add hash factory instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddSingleton<IHashFactory, HashFactory>();
    }
    ```

> 注入 IHashFactory 並建立 IHashAlgorithm 來加解密資料。

2. Create IHashAlgorithm with injected IHashFactory to hash your data.
