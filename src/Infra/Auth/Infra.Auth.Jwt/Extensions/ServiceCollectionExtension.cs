using Infra.Auth.Jwt.Token;
using Infra.Core.Auth.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Auth.Jwt.Extensions;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Add jwt authenticator
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuthenticator(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<AccessTokenHelper>();
        services.AddSingleton<RefreshTokenHelper>();
        services.AddScoped<IAuthenticator, JwtAuthenticator>();

        return services;
    }
}
