using System;
using System.Text;
using Infra.Auth.Jwt.DemoApi.Abstractions.Services;
using Infra.Auth.Jwt.DemoApi.Extensions;
using Infra.Auth.Jwt.DemoApi.Services;
using Infra.Auth.Jwt;
using Infra.Auth.Jwt.Extensions;
using Infra.Auth.Jwt.Token;
using Infra.Core.Auth.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AuthJwtConfig = Infra.Auth.Jwt.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthJwtConfig.Settings>(settings => builder.Configuration.GetSection(AuthJwtConfig.Settings.SectionName).Bind(settings));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

#region Service

builder.Services.AddSingleton<AccessTokenHelper>();
builder.Services.AddSingleton<RefreshTokenHelper>();
builder.Services.AddScoped<IAuthenticator, JwtAuthenticator>();
builder.Services.AddScoped<IUserService, UserService>();

#endregion

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

#region Custom

app.UseInvalidTokenHandleMiddleware();

app.UseExceptionHandleMiddleware();

#endregion

app.MapControllers();

#endregion

app.Run();
