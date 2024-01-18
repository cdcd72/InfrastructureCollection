using Grpc.Core;
using GrpcFileServer.Configuration;
using GrpcFileServer.Configuration.Validators;
using Infra.Core.Extensions;
using GrpcFileService;
using Infra.Core.FileAccess.Abstractions;
using Microsoft.Extensions.Options;

#pragma warning disable 1998

namespace GrpcFileServer.Services;

public class DirectoryService : DirectoryTransfer.DirectoryTransferBase
{
    private readonly ILogger<FileService> logger;
    private readonly Settings settings;
    private readonly IFileAccess fileAccess;

    public DirectoryService(ILogger<FileService> logger, IOptions<Settings> settings, IFileAccess fileAccess)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        this.fileAccess = fileAccess;
    }

    public override async Task<CreateDirectoryResponse> CreateDirectory(
        CreateDirectoryRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var directoryPath = Path.Combine(settings.Root, request.DirectoryName);
        var reply = new CreateDirectoryResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently create directory {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            fileAccess.CreateDirectory(directoryPath);

            logger.Information($"【{mark}】Create directory completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Create directory unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<IsExistDirectoryResponse> IsExistDirectory(
        IsExistDirectoryRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var directoryPath = Path.Combine(settings.Root, request.DirectoryName);
        var reply = new IsExistDirectoryResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently check directory {directoryPath} exist, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            reply.Status = fileAccess.DirectoryExists(directoryPath);

            logger.Information($"【{mark}】Check directory exist completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Check directory exist unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<GetFilesResponse> GetFiles(
        GetFilesRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var directoryPath = Path.Combine(settings.Root, request.DirectoryName);
        var reply = new GetFilesResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently get files from {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            if (Enum.TryParse<SearchOption>(request.SearchOption, out var searchOption))
                reply.FileNames.AddRange(fileAccess.GetFiles(directoryPath, request.SearchPattern, searchOption));

            logger.Information($"【{mark}】Get files completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Get files unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<DeleteDirectoryResponse> DeleteDirectory(
        DeleteDirectoryRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var directoryPath = Path.Combine(settings.Root, request.DirectoryName);
        var reply = new DeleteDirectoryResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently delete directory {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            fileAccess.DeleteDirectory(directoryPath, request.Recursive);

            logger.Information($"【{mark}】Delete directory completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Delete directory unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<GetSubDirectoriesResponse> GetSubDirectories(
        GetSubDirectoriesRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var directoryPath = Path.Combine(settings.Root, request.DirectoryName);
        var reply = new GetSubDirectoriesResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently get subdirectories from {directoryPath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            if (Enum.TryParse<SearchOption>(request.SearchOption, out var searchOption))
                reply.DirectoryNames.AddRange(fileAccess.GetSubDirectories(directoryPath, request.SearchPattern, searchOption));

            logger.Information($"【{mark}】Get subdirectories completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Get subdirectories unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<DirectoryCompressResponse> DirectoryCompress(
        DirectoryCompressRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var directoryPath = Path.Combine(settings.Root, request.DirectoryName);
        var zipFilePath = Path.Combine(settings.Root, request.ZipFileName);
        var compressionLevel = request.CompressionLevel;
        var reply = new DirectoryCompressResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently compress directory {directoryPath} to {zipFilePath} with compression level {compressionLevel}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            fileAccess.DirectoryCompress(directoryPath, zipFilePath, compressionLevel);

            logger.Information($"【{mark}】Compress directory completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Compress directory unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }
}