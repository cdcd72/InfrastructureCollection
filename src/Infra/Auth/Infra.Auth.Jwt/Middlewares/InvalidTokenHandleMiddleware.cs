using Infra.Auth.Jwt.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Infra.Auth.Jwt.Middlewares;

public class InvalidTokenHandleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public InvalidTokenHandleMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        _cache.TryGetValue(CacheKeys.TokenBlackList, out List<string> tokenBlackList);

        if (tokenBlackList is not null)
        {
            var accessToken =
                await httpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

            if (accessToken is not null)
            {
                if (tokenBlackList.Any(token => token == accessToken))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    await httpContext.Response.CompleteAsync();
                }
                else
                {
                    await _next(httpContext);
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
        else
        {
            await _next(httpContext);
        }
    }
}
