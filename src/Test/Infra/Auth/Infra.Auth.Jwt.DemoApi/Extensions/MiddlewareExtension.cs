using Infra.Auth.Jwt.DemoApi.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Infra.Auth.Jwt.DemoApi.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<ExceptionHandleMiddleware>();
}
