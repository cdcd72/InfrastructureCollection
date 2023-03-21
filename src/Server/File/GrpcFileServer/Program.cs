using GrpcFileServer.Services;
using Infra.Core.FileAccess.Abstractions;
using Infra.FileAccess.Physical;
using Serilog;
using GrpcFileServerConfig = GrpcFileServer.Configuration;
using PhysicalFileAccessConfig = Infra.FileAccess.Physical.Configuration;

#pragma warning disable CA1852

try
{
    var builder = WebApplication.CreateBuilder(args);

    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .Build();

    #region Serilog related configuration

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .CreateLogger();

    builder.Host.UseSerilog();

    #endregion

    builder.Configuration.AddConfiguration(config);

    builder.Services
        // Grpc 檔案伺服器設定
        .Configure<GrpcFileServerConfig.Settings>(settings => builder.Configuration.GetSection(GrpcFileServerConfig.Settings.SectionName).Bind(settings))
        // 實體檔案存取設定
        .Configure<PhysicalFileAccessConfig.Settings>(settings => builder.Configuration.GetSection(PhysicalFileAccessConfig.Settings.SectionName).Bind(settings));

    builder.Services.AddSingleton<IFileAccess, PhysicalFileAccess>();

    builder.Services.AddGrpc();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.MapGrpcService<DirectoryService>();
    app.MapGrpcService<FileService>();
    app.MapGet("/", async context => await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "GrpcFileServer terminated unexpectedly.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
