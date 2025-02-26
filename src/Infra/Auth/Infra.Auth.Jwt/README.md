# Infra.Auth.Jwt

透過 Microsoft.AspNetCore.Authentication.JwtBearer 實現 JWT 授權機制。  
Implement JWT authorization mechanism through Microsoft.AspNetCore.Authentication.JwtBearer.

## 使用方式

### Producer (the server that generates the token).

> 配置 appsettings.json

1. Configure appsettings.json

   ```json
   {
     "Auth": {
       "Jwt": {
         "AccessTokenSecret": "your_secret",
         "AccessTokenExpirationMinutes": 480,
         "RefreshTokenSecret": "your_secret",
         "RefreshTokenExpirationMinutes": 600,
         "Issuer": "some_issuer",
         "Audience": "some_audience"
       }
     }
   }
   ```

   - AccessTokenSecret：Access token secret (string determined by you).
   - AccessTokenExpirationMinutes：Access token expiration time (minutes).
   - RefreshTokenSecret：Refresh token secret (string determined by you).
   - RefreshTokenExpirationMinutes：Refresh token expiration time (minutes, must be greater than the AccessTokenExpirationMinutes setting value).
   - Issuer：Issuer (defines who issued the token, string determined by you).
   - Audience：Audience (defines who the token is intended for, string determined by you).

> 新增相關實例至 DI 容器中。

2. Add relevant instances to the DI container.

   ```csharp
   using AuthJwtConfig = Infra.Auth.Jwt.Configuration;

   // ...

   builder.Services.Configure<AuthJwtConfig.Settings>(settings => builder.Configuration.GetSection(AuthJwtConfig.Settings.SectionName).Bind(settings));

   builder.Services.AddJwtAuthenticator();
   ```

> 注入 `IAuthenticator` 來使用相關方法（ex. 產生權杖）。

3. Inject `IAuthenticator` to use relevant methods (e.g., generate token).

### Receiver (the server that receives the token).

> 配置 appsettings.json

1. Configure appsettings.json

   ```json
   {
     "Auth": {
       "Jwt": {
         "AccessTokenSecret": "your_secret",
         "Issuer": "some_issuer",
         "Audience": "some_audience"
       }
     }
   }
   ```

   - AccessTokenSecret：Access token secret (must match the producer's setting if `ValidateIssuerSigningKey = true`).
   - Issuer：Issuer (must match the producer's setting if `ValidateIssuer = true`).
   - Audience：Audience (must match the producer's setting if `ValidateAudience = true`).

> 新增相關實例至 DI 容器中。

2. Add relevant instances to the DI container.

   ```csharp
   // In the top right corner of Swagger, there will be an 'Authorize' button to facilitate subsequent API testing...
   builder.Services.AddSwaggerGen(options =>
   {
      options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
      {
         Type = SecuritySchemeType.ApiKey,
         Scheme = JwtBearerDefaults.AuthenticationScheme,
         In = ParameterLocation.Header,
         Name = "Authorization",
         Description = "JWT Authorization header using the Bearer scheme."
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
         {
            new OpenApiSecurityScheme
            {
               Reference = new OpenApiReference
               {
                  Type = ReferenceType.SecurityScheme,
                  Id = JwtBearerDefaults.AuthenticationScheme
               }
            },
            Array.Empty<string>()
         }
      });
   });

   builder.Services.AddMemoryCache();

   // Add authentication policy.
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Auth:Jwt:AccessTokenSecret"))),
        ValidIssuer = builder.Configuration.GetValue<string>("Auth:Jwt:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("Auth:Jwt:Audience"),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true
    });

   var app = builder.Build();

   // https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order
   #region Middleware

   // ...

   if (app.Environment.IsProduction())
   {
      app.UseHsts();
      app.UseHttpsRedirection();
   }

   // 1. Authenticate first.
   app.UseAuthentication();

   // 2. Authorize afterward.
   app.UseAuthorization();

   #region Custom

   // 3. Avoid access with invalid tokens.
   app.UseInvalidTokenHandleMiddleware();

   // ...

   #endregion

   app.MapControllers();

   #endregion

   app.Run();
   ```

> 對需要經過授權的 Controller 動作（Action）加上 `[Authorize]` 屬性。

3. Add the `[Authorize]` attribute to Controller actions that require authorization.

   ```csharp
   public class DemoController : BaseApiController
   {
      [HttpGet("situation/1")]
      public string GetNotLimited()
         => "Not Authorize";

      // General usage scenario.
      [Authorize]
      [HttpGet("situation/2")]
      public string GetLimited()
         => "Authorized";

      // Constrained permission scenario (when generating a token, the provided claims must include a specific role).
      [Authorize(Roles = AuthConstant.Admin)]
      [HttpGet("situation/3")]
      public string GetLimitedWithAdminRole()
         => "Authorized With Role = Admin";

      // Constrained permission scenario (when generating a token, the provided claims must include a specific role).
      [Authorize(Roles = $"{AuthConstant.Admin},{AuthConstant.User}")]
      [HttpGet("situation/4")]
      public string GetLimitedWithAdminOrUserRole()
         => "Authorized With Role = Admin or User";
   }
   ```
