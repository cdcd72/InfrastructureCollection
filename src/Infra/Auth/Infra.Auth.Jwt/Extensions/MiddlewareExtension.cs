using Infra.Auth.Jwt.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Infra.Auth.Jwt.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseInvalidTokenHandleMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<InvalidTokenHandleMiddleware>();
}
