# Infra.FileAccess.Ftp

透過 FluentFTP 實現 FTP 檔案存取機制。  
Implement ftp file access mechanism with FluentFTP.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    {
        "File": {
            "Ftp": {
                "Host": "",
                "Port": 0,
                "User": "",
                "Password": ""
            }
        }
    }
    ```

    - Host：Ftp server address
    - Port：Ftp server port
    - User：Ftp user
    - Password：Ftp password

> 新增 FTP 檔案存取實例至 DI 容器中。

2. Add Ftp file access instance to DI container

    ```csharp
    builder.Services.AddLogging();

    // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
    builder.Services.Configure<Settings>(settings => builder.Configuration.GetSection(Settings.SectionName).Bind(settings));

    builder.Services.AddSingleton<IFileAccess, FtpFileAccess>();
    ```

> 注入 `IFileAccess` 來使用 FTP 檔案存取。

3. Inject `IFileAccess` to use ftp file access.
