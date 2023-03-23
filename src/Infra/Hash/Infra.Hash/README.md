# Infra.Hash

實現雜湊機制。  
Implement hash mechanism.

## How to use

> 新增雜湊工廠實例至 DI 容器中。

1. Add hash factory instance to DI container

    ```csharp
    builder.Services.AddSingleton<IHashFactory, HashFactory>();

    // or Hash-based Message Authentication Code, HMAC

    builder.Services.AddSingleton<IHmacFactory, HmacFactory>();
    ```

> 注入 `IHashFactory` (or `IHmacAlgorithm`) 並建立 `IHashAlgorithm` (or `IHmacFactory`) 來雜湊資料。

2. Create `IHashAlgorithm`(or `IHmacAlgorithm`) with injected `IHashFactory`(or `IHmacFactory`) to hash your data.
