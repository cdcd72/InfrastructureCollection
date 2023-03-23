# Infra.FileAccess.Sftp

透過 SSH.NET 實現 SFTP 檔案存取機制。  
Implement sftp file access mechanism with SSH.NET.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    {
        "File": {
            "Sftp": {
                "Host": "",
                "Port": 0,
                "User": "",
                "Password": ""
            }
        }
    }
    ```

    - Host：Sftp server address
    - Port：Sftp server port
    - User：Sftp user
    - Password：Sftp password

> 新增 SFTP 檔案存取實例至 DI 容器中。

2. Add Sftp file access instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddLogging();

        // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
    services.Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

        services.AddSingleton<IFileAccess, SftpFileAccess>();
    }
    ```

> 注入 `IFileAccess` 來使用 SFTP 檔案存取。

3. Inject `IFileAccess` to use sftp file access.
