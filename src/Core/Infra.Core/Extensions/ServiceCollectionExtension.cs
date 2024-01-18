using Infra.Core.Background;
using Infra.Core.Background.Abstractions;
using Infra.Core.Background.Models;
using Infra.Core.Configuration;
using Infra.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Core.Extensions;

public static class ServiceCollectionExtension
{
    #region Config

    public static IServiceCollection AddSingletonConfig<TConfig>(this IServiceCollection services, IConfiguration config) where TConfig : class
    {
        services.AddSingleton(_ => BindConfigInstance<TConfig>(config));
        return services;
    }

    public static IServiceCollection AddScopedConfig<TConfig>(this IServiceCollection services, IConfiguration config) where TConfig : class
    {
        services.AddScoped(_ => BindConfigInstance<TConfig>(config));
        return services;
    }

    public static IServiceCollection AddTransientConfig<TConfig>(this IServiceCollection services, IConfiguration config) where TConfig : class
    {
        services.AddTransient(_ => BindConfigInstance<TConfig>(config));
        return services;
    }

    public static IServiceCollection AddConfig<TConfig>(this IServiceCollection services, IConfiguration config, ServiceLifetime lifetime) where TConfig : class
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(_ => BindConfigInstance<TConfig>(config));
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(_ => BindConfigInstance<TConfig>(config));
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(_ => BindConfigInstance<TConfig>(config));
                break;
            default:
                throw new UnexpectedEnumValueException($"Value of enum {typeof(ServiceLifetime)}: {nameof(ServiceLifetime)} is not supported.");
        }

        return services;
    }

    #endregion

    #region HttpClient

    public static IServiceCollection AddNamedHttpClient(this IServiceCollection services, IConfiguration config)
    {
        var serviceSettings = config.GetSection(BackgroundTaskQueueOptions.SectionName).Get<ServiceSettings[]>();

        if (serviceSettings is not null)
        {
            foreach (var serviceSetting in serviceSettings)
            {
                services.AddHttpClient(serviceSetting.Name, client =>
                {
                    client.BaseAddress = new Uri(serviceSetting.Url);
                    if (serviceSetting.Timeout.HasValue)
                        client.Timeout = TimeSpan.FromSeconds(serviceSetting.Timeout.Value);
                });
            }
        }

        return services;
    }

    public static HttpClient GetNamedHttpClient(this IServiceCollection services, string name)
    {
        using var sp = services.BuildServiceProvider();

        var clientFactory = sp.GetRequiredService<IHttpClientFactory>();

        return clientFactory.CreateClient(name);
    }

    #endregion

    #region Background

    public static IServiceCollection AddBackgroundTaskQueues(this IServiceCollection services, IConfiguration config)
    {
        var taskQueueOptions = config.GetSection(BackgroundTaskQueueOptions.SectionName).Get<BackgroundTaskQueueOptions[]>();

        if (taskQueueOptions is not null)
        {
            foreach (var taskQueueOption in taskQueueOptions)
            {
                services.AddSingleton<IBackgroundTaskQueue>(new BackgroundTaskQueue(taskQueueOption));
            }
        }

        return services;
    }

    #endregion

    #region Private Method

    private static TConfig BindConfigInstance<TConfig>(IConfiguration section) where TConfig : class
    {
        var instance = Activator.CreateInstance<TConfig>();
        section.Bind(instance);
        return instance;
    }

    #endregion
}
