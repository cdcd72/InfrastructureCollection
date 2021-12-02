using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcFileServer.Common;
using Infra.Core.FileAccess.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrpcFileServer.Services
{
    public class FileService : FileTransfer.FileTransferBase
    {
        private readonly ILogger<FileService> _logger;
        private readonly Env _env;
        private readonly IFileAccess _fileAccess;

        public FileService(ILogger<FileService> logger, IConfiguration config, IFileAccess fileAccess)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = new Env(config);
            _fileAccess = fileAccess;
        }

        public override async Task Upload(
            IAsyncStreamReader<UploadRequest> requestStream,
            IServerStreamWriter<UploadResponse> responseStream,
            ServerCallContext context)
        {
            var fileContents = new List<UploadRequest>();

            FileStream fs = null;
            var startTime = DateTime.Now;
            var mark = string.Empty;
            var rootDirectoryPath = _env.DirectoryRoot;
            var subDirectoryPath = string.Empty;
            var fileName = string.Empty;
            var savePath = string.Empty;

            if (!_fileAccess.DirectoryExists(rootDirectoryPath))
                _fileAccess.CreateDirectory(rootDirectoryPath);

            try
            {
                while (await requestStream.MoveNext())
                {
                    var reply = requestStream.Current;

                    mark = reply.Mark;

                    // All file transfer completed. (Block = -2)
                    if (reply.Block == -2)
                    {
                        _logger.LogInformation($"【{mark}】File upload completed. SpentTime:{DateTime.Now - startTime}");
                        break;
                    }
                    // file transfer canceled. (Block = -1)
                    else if (reply.Block == -1)
                    {
                        _logger.LogInformation($"【{mark}】File【{reply.Filename}】upload canceled!");

                        #region Clean file and reset variable

                        fileContents.Clear();
                        fs?.Close();

                        if (!string.IsNullOrEmpty(savePath) && _fileAccess.FileExists(savePath))
                            _fileAccess.DeleteFile(savePath);

                        savePath = string.Empty;

                        #endregion

                        break;
                    }
                    // file transfer completed. (Block = 0)
                    else if (reply.Block == 0)
                    {
                        #region Write file and reset variable

                        if (fileContents.Any())
                        {
                            fileContents.OrderBy(c => c.Block).ToList().ForEach(c => c.Content.WriteTo(fs));
                            fileContents.Clear();
                        }

                        fs?.Close();

                        savePath = string.Empty;

                        #endregion

                        // Tell client file transfer completed.
                        await responseStream.WriteAsync(new UploadResponse
                        {
                            Filename = reply.Filename,
                            Mark = mark
                        });
                    }
                    else
                    {
                        // save path is empty means file probably coming.
                        if (string.IsNullOrEmpty(savePath))
                        {
                            // reply.Filename value may be:
                            // 1. 123.txt
                            // 2. Data\\123.txt
                            subDirectoryPath =
                                Path.Combine(rootDirectoryPath, Path.GetDirectoryName(reply.Filename));
                            fileName = Path.GetFileName(reply.Filename);

                            if (!_fileAccess.DirectoryExists(subDirectoryPath))
                                _fileAccess.CreateDirectory(subDirectoryPath);

                            savePath = Path.Combine(subDirectoryPath, fileName);
                            fs = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
                            _logger.LogInformation($"【{mark}】Currently upload file to {savePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");
                        }

                        // Add current file chunk to list.
                        fileContents.Add(reply);

                        // Collect file chunks then write into file stream. (file chunk size decide by client code...)
                        if (fileContents.Count >= _env.ChunkBufferCount)
                        {
                            fileContents.OrderBy(c => c.Block).ToList().ForEach(c => c.Content.WriteTo(fs));
                            fileContents.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】File upload unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }
            finally
            {
                fs?.Dispose();
            }
        }

        public override async Task Download(
            DownloadRequest request,
            IServerStreamWriter<DownloadResponse> responseStream,
            ServerCallContext context)
        {
            var successFileNames = new List<string>();

            FileStream fs = null;
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var chunkSize = _env.ChunkSize;
            var buffer = new byte[chunkSize];
            var fileName = request.Filename;
            var filePath = Path.Combine(_env.DirectoryRoot, fileName);
            var reply = new DownloadResponse
            {
                Filename = fileName,
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently download file from {filePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                if (_fileAccess.FileExists(filePath))
                {
                    fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, chunkSize, useAsync: true);

                    var readTimes = 0;

                    while (true)
                    {
                        if (context.CancellationToken.IsCancellationRequested)
                        {
                            _logger.LogInformation($"【{mark}】File【{filePath}】download canceled!");
                            break;
                        }

                        var readSise = fs.Read(buffer, 0, buffer.Length);

                        // Transfer file chunk to client.
                        if (readSise > 0)
                        {
                            reply.Block = ++readTimes;
                            reply.Content = Google.Protobuf.ByteString.CopyFrom(buffer, 0, readSise);
                            await responseStream.WriteAsync(reply);
                        }
                        // Transfer is completed.
                        else
                        {
                            reply.Block = 0;
                            reply.Content = Google.Protobuf.ByteString.Empty;
                            await responseStream.WriteAsync(reply);
                            successFileNames.Add(fileName);
                            break;
                        }
                    }

                    fs?.Close();
                }
                else
                {
                    _logger.LogInformation($"【{mark}】File【{filePath}】not exists!");
                    reply.Block = -1; // -1 means file not exists, like file transfer canceled situation.
                    await responseStream.WriteAsync(reply);
                }

                if (!context.CancellationToken.IsCancellationRequested)
                {
                    // Tell client file transfer completed.
                    await responseStream.WriteAsync(new DownloadResponse
                    {
                        Filename = string.Empty,
                        Block = -2, // -2 means all file chunk transfer completed.
                        Content = Google.Protobuf.ByteString.Empty,
                        Mark = mark
                    });

                    _logger.LogInformation($"【{mark}】File download completed. SpentTime:{DateTime.Now - startTime}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】File download unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }
            finally
            {
                fs?.Dispose();
            }
        }

        public override async Task<IsExistResponse> IsExist(
            IsExistRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var fileName = request.Filename;
            var filePath = Path.Combine(_env.DirectoryRoot, fileName);
            var reply = new IsExistResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently check file {filePath} exist, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                reply.Status = _fileAccess.FileExists(filePath);

                _logger.LogInformation($"【{mark}】Check file exist completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Check file exist unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }

        public override async Task<DeleteResponse> Delete(
            DeleteRequest request,
            ServerCallContext context)
        {
            var startTime = DateTime.Now;
            var mark = request.Mark;
            var fileName = request.Filename;
            var filePath = Path.Combine(_env.DirectoryRoot, fileName);
            var reply = new DeleteResponse
            {
                Mark = mark
            };

            try
            {
                _logger.LogInformation($"【{mark}】Currently delete file {filePath}, UtcNow:{DateTime.UtcNow:HH:mm:ss:ffff}");

                _fileAccess.DeleteFile(filePath);

                _logger.LogInformation($"【{mark}】Delete file completed. SpentTime:{DateTime.Now - startTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"【{mark}】Delete file unexpected exception happened.({ex.GetType()}):{ex.Message}");
            }

            return reply;
        }
    }
}
