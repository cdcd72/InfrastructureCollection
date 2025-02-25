using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infra.Auth.Jwt.DemoApi.Middlewares;

public class ExceptionHandleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandleMiddleware> _logger;

    public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await ProcessExceptionAsync(httpContext, ex);
        }
    }

    private async Task ProcessExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError($"Api unexpected error occurred! message:{exception.Message}");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.CompleteAsync();
    }
}
