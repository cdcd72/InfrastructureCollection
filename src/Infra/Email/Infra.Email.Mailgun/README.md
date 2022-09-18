# Infra.Email.Mailgun

透過 Mailgun API 實現送信機制。  
Implement send mail mechanism with Mailgun API.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "Email": {
        "Mailgun": {
            "ApiBaseUrl": ""
            "ApiKey": ""
        }
    }
    ```

    - ApiBaseUrl：API base url from Mailgun (ex: https://api.mailgun.net/v3/your-domainname)
    - ApiKey：API key from Mailgun (ex: key-83256...)

> 新增 Mailgun 客戶端實例至 DI 容器中。

2. Add Mailgun client instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddLogging();

        service.AddHttpClient();

        // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
        services.Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

        services.AddSingleton<IMailClient, MailgunClient>();
    }
    ```

> 注入 IMailClient 來送信。

3. Inject IMailClient to send mail.
