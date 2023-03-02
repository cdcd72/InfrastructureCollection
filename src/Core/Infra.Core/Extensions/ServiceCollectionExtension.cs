using Infra.Core.Configuration;
using Infra.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Core.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSingletonConfig<TConfig>(this IServiceCollection services, IConfiguration section) where TConfig : class
    {
        services.AddSingleton(sp => BindConfigInstance<TConfig>(section));
        return services;
    }

    public static IServiceCollection AddScopedConfig<TConfig>(this IServiceCollection services, IConfiguration section) where TConfig : class
    {
        services.AddScoped(sp => BindConfigInstance<TConfig>(section));
        return services;
    }

    public static IServiceCollection AddTransientConfig<TConfig>(this IServiceCollection services, IConfiguration section) where TConfig : class
    {
        services.AddTransient(sp => BindConfigInstance<TConfig>(section));
        return services;
    }

    public static IServiceCollection AddConfig<TConfig>(this IServiceCollection services, IConfiguration section, ServiceLifetime lifetime) where TConfig : class
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(sp => BindConfigInstance<TConfig>(section));
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(sp => BindConfigInstance<TConfig>(section));
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(sp => BindConfigInstance<TConfig>(section));
                break;
            default:
                throw new UnexpectedEnumValueException($"Value of enum {typeof(ServiceLifetime)}: {nameof(ServiceLifetime)} is not supported.");
        }

        return services;
    }

    public static IServiceCollection AddNamedHttpClient(this IServiceCollection services, IEnumerable<ServiceSettings> serviceSettings)
    {
        foreach (var serviceSetting in serviceSettings)
        {
            services.AddHttpClient(serviceSetting.Name, client =>
            {
                client.BaseAddress = new Uri(serviceSetting.Url);
                if (serviceSetting.Timeout.HasValue)
                    client.Timeout = TimeSpan.FromMinutes(serviceSetting.Timeout.Value);
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

    #region Private Method

    private static TConfig BindConfigInstance<TConfig>(IConfiguration section) where TConfig : class
    {
        var instance = Activator.CreateInstance<TConfig>();
        section.Bind(instance);
        return instance;
    }

    #endregion
}
