# Infra.Crypto

實現加解密機制。  
Implement encryption and decryption mechanism.

## How to use

> 新增加密工廠實例至 DI 容器中。

1. Add crypto factory instance to DI container

    ```csharp
    builder.Services.AddSingleton<ICryptoFactory, CryptoFactory>();
    ```

> 注入 ICryptoFactory 並建立 ICryptoAlgorithm 來加解密資料。

2. Create ICryptoAlgorithm with injected ICryptoFactory to encrypt or decrypt your data.
