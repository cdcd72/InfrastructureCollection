using GrpcFileServer.Services;
using Infra.Core.FileAccess.Abstractions;
using Infra.FileAccess.Physical;
using Infra.FileAccess.Physical.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace GrpcFileServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddNLog("Nlog.config"));

            services.Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

            services.AddSingleton<IFileAccess, PhysicalFileAccess>();

            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<DirectoryService>();
                endpoints.MapGrpcService<FileService>();

                endpoints.MapGet("/", async context => await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"));
            });
        }
    }
}
