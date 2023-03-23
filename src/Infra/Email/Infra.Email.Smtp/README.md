# Infra.Email.Smtp

透過 MailKit 實現送信機制。  
Implement send mail mechanism with MailKit.

## How to use

> 配置 appsettings.json

1. Configure appsettings.json

    ```json
    "Email": {
        "Smtp": {
          "Host": "",
          "Port": 0,
          "Account": "",
          "Password": "",
          "EnableSsl": false
        }
    }
    ```

    - Host：SMTP host
    - Port：SMTP port
    - Account：Account for authenticate
    - Password：Password for authenticate
    - EnableSsl: Connect to SMTP with https

> 新增 SMTP 客戶端實例至 DI 容器中。

2. Add SMTP client instance to DI container

    ```csharp
    builder.Services.AddLogging();

    // Section name from settings is defaulted, you can change your prefer naming, but field structure must be the same!
    builder.Services.Configure<Settings>(settings => builder.Configuration.GetSection(Settings.SectionName).Bind(settings));

    builder.Services.AddSingleton<IMailClient, SmtpClient>();
    ```

> 注入 `IMailClient` 來送信。

3. Inject `IMailClient` to send mail.
