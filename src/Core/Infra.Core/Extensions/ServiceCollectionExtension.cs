using Infra.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Core.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddNamedHttpClient(this IServiceCollection services, IEnumerable<ServiceSettings> serviceSettings)
    {
        foreach (var serviceSetting in serviceSettings)
        {
            services.AddHttpClient(serviceSetting.Name, client =>
            {
                client.BaseAddress = new Uri(serviceSetting.Url);
                if (serviceSetting.Timeout.HasValue)
                    client.Timeout = TimeSpan.FromMilliseconds(serviceSetting.Timeout.Value);
            });
        }

        return services;
    }

    public static HttpClient GetNamedHttpClient(this IServiceCollection services, string name)
    {
        using var sp = services.BuildServiceProvider();

        var clientFactory = sp.GetRequiredService<IHttpClientFactory>();

        return clientFactory.CreateClient(name);
    }
}
