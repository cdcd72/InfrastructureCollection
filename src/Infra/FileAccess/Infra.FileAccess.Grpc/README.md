# Infra.FileAccess.Grpc ( gRPC client )

透過 gRPC 實現檔案存取機制。  
Implement file access mechanism with gRPC.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "File": {
        "Grpc": {
            "Client": {
                "ServerAddress": "https://localhost:5001",
                "ChunkSize": 1048576,
                "ChunkBufferCount": 20
            }
        }
    }
    ```

    - ServerAddress：gRPC file server address
    - ChunkSize：File chunk size for read file
    - ChunkBufferCount：File chunk buffer for write file

> 新增 gRPC 檔案存取實例至 DI 容器中。

2. Add gRPC file access instance to DI container

    ```csharp
    builder.Services.AddLogging();

    // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
    builder.Services.Configure<Settings>(settings => builder.Configuration.GetSection(Settings.SectionName).Bind(settings));

    builder.Services.AddSingleton<IFileAccess, GrpcFileAccess>();
    ```

> 注入 IFileAccess 來使用 gRPC 檔案存取。

3. Inject IFileAccess to use gRPC file access.
