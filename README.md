# Infrastructure Collection

收集過往實作過的基礎設施。  
Collect the infrastructure that has been implemented in the past.

做法上可能不是最好，但會希望能越來越好！  
The practice may not be the best, but I hope to get better and better!

## Implemented infrastructure list

> All infrastructure are base on **[Infra.Core](src/Core/Infra.Core)**.

- Cache
  - [Infra.Caching.Memory](src/Infra/Cache/Infra.Caching.Memory)
    - Implement with [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory)
  - [Infra.Caching.Redis](src/Infra/Cache/Infra.Caching.Redis)
    - Implement with [Microsoft.Extensions.Caching.StackExchangeRedis](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis)
- Crypto
  - [Infra.Crypto](src/Infra/Crypto/Infra.Crypto)
- Email
  - [Infra.Email.Smtp](src/Infra/Email/Infra.Email.Smtp)
    - Implement with [MailKit](https://github.com/jstedfast/MailKit)
- EventBus
  - [Infra.EventBus.RabbitMQ](src/Infra/EventBus/Infra.EventBus.RabbitMQ) :point_right: [Demo](https://github.com/cdcd72/EventBus.RabbitMQ.Demo)
    - Implement with [Autofac](https://github.com/autofac/Autofac) - Prevent memory leak and better to control object life time.
    - Implement with [Polly](https://github.com/App-vNext/Polly) - Retry mechanism.
- FileAccess
  - [Infra.FileAccess.Physical](src/Infra/FileAccess/Infra.FileAccess.Physical)
  - [Infra.FileAccess.Grpc](src/Infra/FileAccess/Infra.FileAccess.Grpc) :point_right: [Demo](https://github.com/cdcd72/Grpc.FileTransfer.Demo)
- Hash
  - [Infra.Hash](src/Infra/Hash/Infra.Hash)
- Time
  - [Infra.Time](src/Infra/Time/Infra.Time)
- Server
  - [GrpcFileServer](src/Server/File/GrpcFileServer)
    - Collocate with **Infra.FileAccess.Grpc**.
- ...（Keep it up.）

## Referenced github repository

- [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)

## Contribute & Bug report

Please open up an issue on GitHub before you put a lot efforts on pull request.

## License

MIT license
