using Grpc.Core;
using GrpcFileServer.Configuration;
using GrpcFileServer.Configuration.Validators;
using Infra.Core.Extensions;
using GrpcFileService;
using Infra.Core.FileAccess.Abstractions;
using Microsoft.Extensions.Options;

#pragma warning disable 1998

namespace GrpcFileServer.Services;

public class FileService : FileTransfer.FileTransferBase
{
    private readonly ILogger<FileService> logger;
    private readonly Settings settings;
    private readonly IFileAccess fileAccess;

    public FileService(ILogger<FileService> logger, IOptions<Settings> settings, IFileAccess fileAccess)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        this.fileAccess = fileAccess;
    }

    public override async Task UploadFile(
        IAsyncStreamReader<UploadFileRequest> requestStream,
        IServerStreamWriter<UploadFileResponse> responseStream,
        ServerCallContext context)
    {
        var fileContents = new List<UploadFileRequest>();

        FileStream fs = null;
        var startTime = DateTime.Now;
        var mark = string.Empty;
        var rootDirectoryPath = settings.Root;
        var savePath = string.Empty;

        if (!fileAccess.DirectoryExists(rootDirectoryPath))
            fileAccess.CreateDirectory(rootDirectoryPath);

        try
        {
            while (await requestStream.MoveNext())
            {
                var reply = requestStream.Current;

                mark = reply.Mark;

                // All file transfer completed. (Block = -2)
                if (reply.Block == -2)
                {
                    logger.Information($"【{mark}】File upload completed. SpentTime:{DateTime.Now - startTime}");
                    break;
                }
                // file transfer canceled. (Block = -1)
                else if (reply.Block == -1)
                {
                    logger.Information($"【{mark}】File【{reply.FileName}】upload canceled!");

                    #region Clean file and reset variable

                    fileContents.Clear();
                    fs?.Close();

                    if (!string.IsNullOrEmpty(savePath) && fileAccess.FileExists(savePath))
                        fileAccess.DeleteFile(savePath);

                    savePath = string.Empty;

                    #endregion

                    break;
                }
                // file transfer completed. (Block = 0)
                else if (reply.Block == 0)
                {
                    #region Write file and reset variable

                    if (fileContents.Count != 0)
                    {
                        fileContents.OrderBy(c => c.Block).ToList().ForEach(c => c.Content.WriteTo(fs));
                        fileContents.Clear();
                    }

                    fs?.Close();

                    savePath = string.Empty;

                    #endregion

                    // Tell client file transfer completed.
                    await responseStream.WriteAsync(new UploadFileResponse
                    {
                        FileName = reply.FileName,
                        Mark = mark
                    });
                }
                else
                {
                    // save path is empty means file probably coming.
                    if (string.IsNullOrEmpty(savePath))
                    {
                        // reply.FileName value may be:
                        // 1. 123.txt
                        // 2. Data\\123.txt
                        var subDirectoryPath = Path.Combine(rootDirectoryPath, Path.GetDirectoryName(reply.FileName)!);
                        var fileName = Path.GetFileName(reply.FileName);

                        if (!fileAccess.DirectoryExists(subDirectoryPath))
                            fileAccess.CreateDirectory(subDirectoryPath);

                        savePath = Path.Combine(subDirectoryPath, fileName!);
                        fs = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
                        logger.Information($"【{mark}】Currently upload file to {savePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");
                    }

                    // Add current file chunk to list.
                    fileContents.Add(reply);

                    // Collect file chunks then write into file stream. (file chunk size decide by client code...)
                    if (fileContents.Count >= settings.ChunkBufferCount)
                    {
                        fileContents.OrderBy(c => c.Block).ToList().ForEach(c => c.Content.WriteTo(fs));
                        fileContents.Clear();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】File upload unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }
        finally
        {
            if (fs is not null)
                await fs.DisposeAsync();
        }
    }

    public override async Task DownloadFile(
        DownloadFileRequest request,
        IServerStreamWriter<DownloadFileResponse> responseStream,
        ServerCallContext context)
    {
        FileStream fs = null;
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var chunkSize = settings.ChunkSize;
        var buffer = new byte[chunkSize];
        var fileName = request.FileName;
        var filePath = Path.Combine(settings.Root, fileName);
        var reply = new DownloadFileResponse
        {
            FileName = fileName,
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently download file from {filePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            if (fileAccess.FileExists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, chunkSize, useAsync: true);

                var readTimes = 0;

                while (true)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        logger.Information($"【{mark}】File【{filePath}】download canceled!");
                        break;
                    }

                    var readSize = fs.Read(buffer, 0, buffer.Length);

                    // Transfer file chunk to client.
                    if (readSize > 0)
                    {
                        reply.Block = ++readTimes;
                        reply.Content = Google.Protobuf.ByteString.CopyFrom(buffer, 0, readSize);
                        await responseStream.WriteAsync(reply);
                    }
                    // Transfer is completed.
                    else
                    {
                        reply.Block = 0;
                        reply.Content = Google.Protobuf.ByteString.Empty;
                        await responseStream.WriteAsync(reply);
                        break;
                    }
                }

                fs.Close();
            }
            else
            {
                logger.Information($"【{mark}】File【{filePath}】not exists!");
                reply.Block = -1; // -1 means file not exists, like file transfer canceled situation.
                await responseStream.WriteAsync(reply);
            }

            if (!context.CancellationToken.IsCancellationRequested)
            {
                // Tell client file transfer completed.
                await responseStream.WriteAsync(new DownloadFileResponse
                {
                    FileName = string.Empty,
                    Block = -2, // -2 means all file chunk transfer completed.
                    Content = Google.Protobuf.ByteString.Empty,
                    Mark = mark
                });

                logger.Information($"【{mark}】File download completed. SpentTime:{DateTime.Now - startTime}");
            }
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】File download unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }
        finally
        {
            if (fs is not null)
                await fs.DisposeAsync();
        }
    }

    public override async Task<IsExistFileResponse> IsExistFile(
        IsExistFileRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var filePath = Path.Combine(settings.Root, request.FileName);
        var reply = new IsExistFileResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently check file {filePath} exist, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            reply.Status = fileAccess.FileExists(filePath);

            logger.Information($"【{mark}】Check file exist completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Check file exist unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<DeleteFileResponse> DeleteFile(
        DeleteFileRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var filePath = Path.Combine(settings.Root, request.FileName);
        var reply = new DeleteFileResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently delete file {filePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            fileAccess.DeleteFile(filePath);

            logger.Information($"【{mark}】Delete file completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Delete file unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<GetFileSizeResponse> GetFileSize(
        GetFileSizeRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var filePath = Path.Combine(settings.Root, request.FileName);
        var reply = new GetFileSizeResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently get file {filePath} size, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            reply.Size = fileAccess.GetFileSize(filePath);

            logger.Information($"【{mark}】Get file size completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Get file size unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<MoveFileResponse> MoveFile(
        MoveFileRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var rootDirectoryPath = settings.Root;
        var sourceFilePath = Path.Combine(rootDirectoryPath, request.SourceFileName);
        var destinationFilePath = Path.Combine(rootDirectoryPath, request.DestinationFileName);
        var reply = new MoveFileResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently move file from {sourceFilePath} to {destinationFilePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            fileAccess.MoveFile(sourceFilePath, destinationFilePath, request.Overwrite);

            logger.Information($"【{mark}】Move file completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Move file unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }

    public override async Task<CopyFileResponse> CopyFile(
        CopyFileRequest request,
        ServerCallContext context)
    {
        var startTime = DateTime.Now;
        var mark = request.Mark;
        var rootDirectoryPath = settings.Root;
        var sourceFilePath = Path.Combine(rootDirectoryPath, request.SourceFileName);
        var destinationFilePath = Path.Combine(rootDirectoryPath, request.DestinationFileName);
        var reply = new CopyFileResponse
        {
            Mark = mark
        };

        try
        {
            logger.Information($"【{mark}】Currently copy file from {sourceFilePath} to {destinationFilePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

            fileAccess.CopyFile(sourceFilePath, destinationFilePath, request.Overwrite);

            logger.Information($"【{mark}】Copy file completed. SpentTime:{DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            logger.Error($"【{mark}】Copy file unexpected exception happened.({ex.GetType()}):{ex.Message}");
        }

        return reply;
    }
}
