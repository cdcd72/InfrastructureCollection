# GrpcFileServer ( gRPC server )

透過 gRPC 實現檔案伺服器。  
Implement file server with gRPC.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "File": {
        "Grpc": {
            "Server": {
                "Root": "D:\\Output\\File\\Upload",
                "ChunkSize": 1048576,
                "ChunkBufferCount": 20
            }
        }
    }
    ```

    - Root：Files saved directory
    - ChunkSize：File chunk size for read file
    - ChunkBufferCount：File chunk buffer for write file

> 部署在你偏好的環境即可。

2. Deploy to your perfer environment.
