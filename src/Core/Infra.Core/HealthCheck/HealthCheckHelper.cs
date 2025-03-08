using System.Text.Json;
using Infra.Core.HealthCheck.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infra.Core.HealthCheck;

public static class HealthCheckHelper
{
    private static readonly JsonSerializerOptions JsonSerializerWriteOptions = new()
    {
        WriteIndented = true
    };

    public static Task CreateResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = "application/json";

        var response = new HealthCheckResponse
        {
            Status = $"{result.Status}",
            TotalDuration = $"{result.TotalDuration.TotalSeconds:0:0.00}",
            DependencyServices = result.Entries.Select(service => new DependencyService
            {
                Key = service.Key,
                Status = $"{service.Value.Status}",
                Duration = $"{service.Value.Duration.TotalSeconds:0:0.00}",
                Tags = string.Join(",", service.Value.Tags.ToArray()),
                Exception = service.Value.Exception?.Message
            }).ToArray()
        };

        return httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, JsonSerializerWriteOptions));
    }
}
