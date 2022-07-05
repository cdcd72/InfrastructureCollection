using Infra.Core.Cache.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Infra.Caching.Redis.IntegrationTest
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private IServiceProvider ServiceProvider { get; }

        public Startup(IConfiguration? config = null)
        {
            Configuration = config ?? GetConfiguration();
            ServiceProvider = ConfigureServices(new ServiceCollection());
        }

        public T GetService<T>() => ServiceProvider.GetService<T>()!;

        #region Private Method

        private IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = Configuration.GetValue<string>("Redis:InstanceName");
                options.Configuration = Configuration.GetValue<string>("Redis:Configuration");
            });

            services.AddSingleton<ICache, RedisCache>();

            return services.BuildServiceProvider();
        }

        private static IConfiguration GetConfiguration()
        {
            var releaseJsonSource = new JsonConfigurationSource
            {
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
                Path = "appsettings.json",
                Optional = false,
                ReloadOnChange = true
            };

            return new ConfigurationBuilder()
                .Add(releaseJsonSource)
                .Build();
        }

        #endregion
    }
}
