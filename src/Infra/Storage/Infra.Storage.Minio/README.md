# Infra.Storage.Minio

透過 Minio 實現物件式儲存。  
Implement object storage mechanism with Minio.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "Storage": {
      "Minio": {
        "Endpoint": "",
        "AccessKey": "",
        "SecretKey": "",
        "Timeout": 0,
        "WithSSL": false
      }
    }
    ```

    - Endpoint：Url to object storage service.
    - AccessKey：Access key is the user ID that uniquely identifies your account.
    - SecretKey：Secret key is the password to your account.
    - Timeout：Set timeout for all requests. (Timeout in milliseconds)
    - WithSSL：Connects to object storage service with https.

> 新增 Minio 儲存實例至 DI 容器中。

2. Add minio storage instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
        services.Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

        services.AddSingleton<IObjectStorage, MinioStorage>();
    }
    ```

> 注入 IObjectStorage 來操作物件。

3. Inject IObjectStorage to operate object.
