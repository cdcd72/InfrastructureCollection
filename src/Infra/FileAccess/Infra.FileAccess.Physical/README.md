# Infra.FileAccess.Physical

實現實體檔案存取機制。  
Implement physical file access mechanism.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "File": {
        "Roots": [
            "D:\\Output\\File\\Upload"
        ]
    }
    ```

    - Roots：Root directories for prevent path traversal

> 新增實體檔案存取實例至 DI 容器中。

2. Add physical file access instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
        services.Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

        services.AddSingleton<IFileAccess, PhysicalFileAccess>();
    }
    ```

> 注入 IFileAccess 來使用實體檔案存取。

3. Inject IFileAccess to use physical file access.
