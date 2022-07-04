# Infra.EventBus.RabbitMQ

透過 RabbitMQ 實現事件匯流排機制。  
Implement event bus mechanism with RabbitMQ.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "EventBus": {
        "RabbitMQ": {
            "Connection": {
                "Host": "localhost",
                "Port": 5672,
                "UserName": "",
                "Password": "",
                "RetryCount": 5,
            },
            "QueueName": "Queue",
            "RetryCount": 5
        }
    }
    ```

    - Connection
        - Host：RabbitMQ host name
        - Port：RabbitMQ host port
        - UserName：RabbitMQ user name
        - Password：RabbitMQ user password
        - RetryCount：RabbitMQ connection retry count
    - QueueName：RabbitMQ queue name
    - RetryCount：RabbitMQ event publish retry count

> 註冊事件匯流排

2. Register event bus from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
        services.Configure<ConnectionSettings>(settings => Configuration.GetSection(ConnectionSettings.SectionName).Bind(settings))
                .Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

        // Register event bus
        RegisterEventBus(services);
    }
    ```

    ```csharp
    private static void RegisterEventBus(IServiceCollection services)
    {
        // Add RabbitMQ connection
        services.AddSingleton<IRabbitMQConnection, DefaultRabbitMQConnection>();

        // Add event bus subscriptions manager
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        // Add event bus
        services.AddSingleton<IEventBus, RabbitMQBus>();
    }
    ```

> 訂閱事件

1. Subscribe event from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        // Add event handlers
        services.AddTransient<TriggeredIntegrationEventHandler>();
    }
    ```

    ```csharp
    public void Configure(IApplicationBuilder app)
    {
        // ...

        // Configure event bus
        ConfigureEventBus(app);
    }
    ```

    ```csharp
    private static void ConfigureEventBus(IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

        // Subscribing to TriggeredIntegrationEvent with TriggeredIntegrationEventHandler.
        eventBus.Subscribe<TriggeredIntegrationEvent, TriggeredIntegrationEventHandler>();
    }
    ```

> 發送事件

1. Publish event from controller or other implementation

    ```csharp
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IEventBus _eventBus;

        public DemoController(IEventBus eventBus) => _eventBus = eventBus;

        [Route(nameof(TriggerAsync))]
        [HttpPost]
        public async Task<ActionResult> TriggerAsync(string input)
        {
            var eventMessage = new TriggeredIntegrationEvent(input);

            _eventBus.Publish(eventMessage);

            return Ok();
        }
    }
    ```
