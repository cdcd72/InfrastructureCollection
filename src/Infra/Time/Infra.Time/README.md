# Infra.Time

實現時間包裝器。  
Implement time wrapper.

## How to use

### TimeWrapper

> 新增時間包裝器實例至 DI 容器中。

1. Add time wrapper instance to DI container

   ```csharp
   builder.Services.AddSingleton<ITimeWrapper, TimeWrapper>();
   ```

> 注入 `ITimeWrapper` 來使用時間包裝器。

2. Inject `ITimeWrapper` to use time wrapper.

### TimeSpanHelper

> 配置 appsettings.json

1. Configure appsettings.json

   ```json
   {
     "Time": {
       "ExpressionMatchTimeout": 3000
     }
   }
   ```

   - ExpressionMatchTimeout：Expression match timeout（Unit：ms）

> 新增 TimeSpan Helper 實例至 DI 容器中。

2. Add timeSpan helper to DI container

   ```csharp
   builder.Services.Configure<Settings>(settings => builder.Configuration.GetSection(Settings.SectionName).Bind(settings));

   builder.Services.AddSingleton<ITimeSpanHelper, TimeSpanHelper>();
   ```

   > 注入 `ITimeSpanHelper` 來使用 TimeSpan Helper。

3. Inject `ITimeSpanHelper` to use timeSpan helper。
