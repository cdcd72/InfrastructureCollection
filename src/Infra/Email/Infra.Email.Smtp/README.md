# Infra.Email.Smtp

透過 MailKit 實現送信機制。  
Implement send mail mechanism with MailKit.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "Email": {
        "Host": "",
        "Port": 0,
        "Account": "",
        "Password": ""
    }
    ```

    - Host：SMTP host
    - Port：SMTP port
    - Account：account for authenticate
    - Password：password for authenticate

> 新增 SMTP 客戶端實例至 DI 容器中。

2. Add SMTP client instance to DI container from Startup.cs

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        services.AddLogging();

        services.Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

        services.AddSingleton<IMailClient, SmtpClientInstance>();
    }
    ```

> 注入 IMailClient 來送信。

3. Inject IMailClient to send mail.
