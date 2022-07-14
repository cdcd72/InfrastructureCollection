using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcFileService;
using Infra.Core.Extensions;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Models;
using Infra.FileAccess.Grpc.Configuration;
using Infra.FileAccess.Grpc.Configuration.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;

namespace Infra.FileAccess.Grpc
{
    public class GrpcFileAccess : IFileAccess
    {
        private readonly ILogger<GrpcFileAccess> _logger;
        private readonly Settings _settings;
        private readonly RecyclableMemoryStreamManager _msManager;

        #region Constructor

        public GrpcFileAccess(ILogger<GrpcFileAccess> logger, IOptions<Settings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
            _msManager = GetRecyclableMemoryStreamManager();
        }

        #endregion

        #region Sync Method

        #region Directory

        public void CreateDirectory(string directoryPath) => throw new NotSupportedException();

        public bool DirectoryExists(string directoryPath) => throw new NotSupportedException();

        public string[] GetFiles(string directoryPath, string searchPattern = "", SearchOption searchOption = default) => throw new NotSupportedException();

        public void DeleteDirectory(string directoryPath, bool recursive = true) => throw new NotSupportedException();

        public string[] GetSubDirectories(string directoryPath, string searchPattern = "", SearchOption searchOption = default) => throw new NotSupportedException();

        public void DirectoryCompress(string directoryPath, string zipFilePath) => throw new NotSupportedException();

        public string GetParentPath(string directoryPath) => throw new NotSupportedException();

        public string GetCurrentDirectoryName(string directoryPath) => throw new NotSupportedException();

        #endregion

        public bool FileExists(string filePath) => throw new NotSupportedException();

        public void SaveFile(string filePath, string content) => throw new NotSupportedException();

        public void SaveFile(string filePath, string content, Encoding encoding) => throw new NotSupportedException();

        public void SaveFile(string filePath, byte[] bytes) => throw new NotSupportedException();

        public void DeleteFile(string filePath) => throw new NotSupportedException();

        public long GetFileSize(string filePath) => throw new NotSupportedException();

        public string ReadTextFile(string filePath) => throw new NotSupportedException();

        public string ReadTextFile(string filePath, Encoding encoding) => throw new NotSupportedException();

        public byte[] ReadFile(string filePath) => throw new NotSupportedException();

        public void MoveFile(string sourceFilePath, string destFilePath, bool overwrite = true) => throw new NotSupportedException();

        public void CopyFile(string sourceFilePath, string destFilePath, bool overwrite = true) => throw new NotSupportedException();

        #endregion

        #region Async Method

        #region Directory

        public async Task CreateDirectoryAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetDirectoryClient();
            var progressInfo = new ProgressInfo();
            var directoryName = directoryPath;

            try
            {
                var request = new CreateDirectoryRequest()
                {
                    DirectoryName = directoryName,
                    Mark = mark
                };

                progressInfo.Message = $"Currently create directory【{directoryName}】...";
                progressCallBack?.Invoke(progressInfo);

                await client.CreateDirectoryAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Create directory【{directoryName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = directoryName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Create directory【{directoryName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Create directory【{directoryName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task<bool> DirectoryExistsAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetDirectoryClient();
            var progressInfo = new ProgressInfo();
            var directoryName = directoryPath;

            try
            {
                var request = new IsExistDirectoryRequest()
                {
                    DirectoryName = directoryName,
                    Mark = mark
                };

                progressInfo.Message = $"Currently check directory【{directoryName}】exist...";
                progressCallBack?.Invoke(progressInfo);

                var call = await client.IsExistDirectoryAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Check directory【{directoryName}】exist completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = directoryName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Check directory【{directoryName}】exist canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }

                return call.Status;
            }
            catch (Exception ex)
            {
                _logger.Error($"Check directory【{directoryName}】exist unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetDirectoryClient();
            var progressInfo = new ProgressInfo();
            var directoryName = directoryPath;

            try
            {
                var request = new GetFilesRequest()
                {
                    DirectoryName = directoryName,
                    SearchPattern = searchPattern,
                    SearchOption = $"{searchOption}",
                    Mark = mark
                };

                progressInfo.Message = $"Currently get files from【{directoryName}】...";
                progressCallBack?.Invoke(progressInfo);

                var call = await client.GetFilesAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Get files from【{directoryName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = directoryName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Get files from【{directoryName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }

                return call.FileNames.ToArray();
            }
            catch (Exception ex)
            {
                _logger.Error($"Get files from【{directoryName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task DeleteDirectoryAsync(string directoryPath, bool recursive = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetDirectoryClient();
            var progressInfo = new ProgressInfo();
            var directoryName = directoryPath;

            try
            {
                var request = new DeleteDirectoryRequest()
                {
                    DirectoryName = directoryName,
                    Recursive = recursive,
                    Mark = mark
                };

                progressInfo.Message = $"Currently delete directory【{directoryName}】...";
                progressCallBack?.Invoke(progressInfo);

                await client.DeleteDirectoryAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Delete directory【{directoryName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = directoryName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Delete directory【{directoryName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Delete directory【{directoryName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task<string[]> GetSubDirectoriesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetDirectoryClient();
            var progressInfo = new ProgressInfo();
            var directoryName = directoryPath;

            try
            {
                var request = new GetSubDirectoriesRequest()
                {
                    DirectoryName = directoryName,
                    SearchPattern = searchPattern,
                    SearchOption = $"{searchOption}",
                    Mark = mark
                };

                progressInfo.Message = $"Currently get subdirectories from【{directoryName}】...";
                progressCallBack?.Invoke(progressInfo);

                var call = await client.GetSubDirectoriesAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Get subdirectories from【{directoryName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = directoryName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Get subdirectories from【{directoryName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }

                return call.DirectoryNames.ToArray();
            }
            catch (Exception ex)
            {
                _logger.Error($"Get subdirectories from【{directoryName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task DirectoryCompressAsync(string directoryPath, string zipFilePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetDirectoryClient();
            var progressInfo = new ProgressInfo();
            var directoryName = directoryPath;
            var zipFileName = zipFilePath;

            try
            {
                var request = new DirectoryCompressRequest()
                {
                    DirectoryName = directoryName,
                    ZipFileName = zipFileName,
                    Mark = mark
                };

                progressInfo.Message = $"Currently compress directory【{directoryName}】to【{zipFileName}】...";
                progressCallBack?.Invoke(progressInfo);

                await client.DirectoryCompressAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Compress directory【{directoryName}】to【{zipFileName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = directoryName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Compress directory【{directoryName}】to【{zipFileName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Compress directory【{directoryName}】to【{zipFileName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public Task<string> GetParentPathAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            // Can't implemented, because of some security issue.
            => throw new NotSupportedException();

        public Task<string> GetCurrentDirectoryNameAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            // Can't implemented, because of some security issue.
            => throw new NotSupportedException();

        #endregion

        public async Task<bool> FileExistsAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetFileClient();
            var progressInfo = new ProgressInfo();
            var fileName = filePath;

            try
            {
                var request = new IsExistFileRequest()
                {
                    FileName = fileName,
                    Mark = mark
                };

                progressInfo.Message = $"Currently check file【{fileName}】exist...";
                progressCallBack?.Invoke(progressInfo);

                var call = await client.IsExistFileAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Check file【{fileName}】exist completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = fileName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Check file【{fileName}】exist canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }

                return call.Status;
            }
            catch (Exception ex)
            {
                _logger.Error($"Check file【{fileName}】exist unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task SaveFileAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await SaveFileAsync(filePath, content, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task SaveFileAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await SaveFileAsync(filePath, encoding.GetBytes(content), progressCallBack, cancellationToken);

        public async Task SaveFileAsync(string filePath, byte[] bytes, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var buffer = new byte[_settings.ChunkSize];
            var memory = new Memory<byte>(buffer);
            var (channel, client) = GetFileClient();
            var progressInfo = new ProgressInfo();
            var fileName = filePath;
            using var ms = _msManager.GetStream(bytes) as RecyclableMemoryStream;

            try
            {
                using var call = client.UploadFile(cancellationToken: cancellationToken);

                var request = new UploadFileRequest()
                {
                    FileName = fileName,
                    Mark = mark
                };

                var readTimes = 0;
                var uploadedSize = 0;

                while (true)
                {
                    // Initiative cancel.
                    if (cancellationToken.IsCancellationRequested)
                    {
                        request.Block = -1; // -1 means file transfer canceled.
                        request.Content = Google.Protobuf.ByteString.Empty;
                        await call.RequestStream.WriteAsync(request, cancellationToken);

                        progressInfo.IsCompleted = false;
                        progressInfo.Message = $"File【{fileName}】upload canceled. SpentTime:{DateTime.Now - startTime}";
                        _logger.Information(progressInfo.Message);
                        progressCallBack?.Invoke(progressInfo);
                        break;
                    }

                    var readSize = await ms.ReadAsync(memory, cancellationToken);

                    // Transfer file chunk to server.
                    if (readSize > 0)
                    {
                        request.Block = ++readTimes;
                        request.Content = Google.Protobuf.ByteString.CopyFrom(buffer, 0, readSize);
                        await call.RequestStream.WriteAsync(request, cancellationToken);

                        uploadedSize += readSize;
                        progressInfo.Message = $"File【{fileName}】current upload progress【{uploadedSize}/{ms.Length}】bytes.";
                        progressCallBack?.Invoke(progressInfo);
                    }
                    // Transfer is completed.
                    else
                    {
                        request.Block = 0;
                        request.Content = Google.Protobuf.ByteString.Empty;
                        await call.RequestStream.WriteAsync(request, cancellationToken);

                        // Waiting server response.
                        await call.ResponseStream.MoveNext(cancellationToken);

                        if (call.ResponseStream.Current != null && call.ResponseStream.Current.Mark == mark)
                        {
                            progressInfo.IsCompleted = true;
                            progressInfo.Message = $"File【{fileName}】upload completed. SpentTime:{DateTime.Now - startTime}";
                            progressInfo.Result = fileName;
                            _logger.Information(progressInfo.Message);
                            progressCallBack?.Invoke(progressInfo);
                        }

                        break;
                    }
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    await call.RequestStream.WriteAsync(new UploadFileRequest
                    {
                        Block = -2, // -2 means all file chunk transfer completed.
                        Mark = mark
                    }, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"File【{fileName}】upload unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task DeleteFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetFileClient();
            var progressInfo = new ProgressInfo();
            var fileName = filePath;

            try
            {
                var request = new DeleteFileRequest()
                {
                    FileName = fileName,
                    Mark = mark
                };

                progressInfo.Message = $"Currently delete file【{fileName}】...";
                progressCallBack?.Invoke(progressInfo);

                await client.DeleteFileAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Delete file【{fileName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = fileName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Delete file【{fileName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Delete file【{fileName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task<long> GetFileSizeAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetFileClient();
            var progressInfo = new ProgressInfo();
            var fileName = filePath;

            try
            {
                var request = new GetFileSizeRequest()
                {
                    FileName = fileName,
                    Mark = mark
                };

                progressInfo.Message = $"Currently get file【{fileName}】size...";
                progressCallBack?.Invoke(progressInfo);

                var call = await client.GetFileSizeAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Get file【{fileName}】size completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = fileName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Get file【{fileName}】size canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }

                return call.Size;
            }
            catch (Exception ex)
            {
                _logger.Error($"Get file【{fileName}】size unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task<string> ReadTextFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await ReadTextFileAsync(filePath, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task<string> ReadTextFileAsync(string filePath, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var fileBytes = await ReadFileAsync(filePath, progressCallBack, cancellationToken);

            return encoding.GetString(fileBytes);
        }

        public async Task<byte[]> ReadFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetFileClient();
            var progressInfo = new ProgressInfo();
            var fileName = filePath;
            using var ms = _msManager.GetStream() as RecyclableMemoryStream;

            try
            {
                var request = new DownloadFileRequest()
                {
                    FileName = fileName,
                    Mark = mark
                };

                using var call = client.DownloadFile(request, cancellationToken: cancellationToken);

                var fileContents = new List<DownloadFileResponse>();
                var reaponseStream = call.ResponseStream;

                while (await reaponseStream.MoveNext(cancellationToken))
                {
                    // Initiative cancel.
                    if (cancellationToken.IsCancellationRequested)
                    {
                        progressInfo.IsCompleted = false;
                        progressInfo.Message = $"File【{fileName}】download canceled. SpentTime:{DateTime.Now - startTime}";
                        _logger.Information(progressInfo.Message);
                        progressCallBack?.Invoke(progressInfo);
                        break;
                    }

                    // All file transfer completed. (Block = -2)
                    if (reaponseStream.Current.Block == -2)
                    {
                        // -2 means all file chunk transfer completed.
                        break;
                    }
                    // file transfer canceled or error happened. (Block = -1)
                    else if (reaponseStream.Current.Block == -1)
                    {
                        progressInfo.IsCompleted = false;
                        progressInfo.Message = $"File【{fileName}】download transfer failed. SpentTime:{DateTime.Now - startTime}";
                        _logger.Information(progressInfo.Message);
                        progressCallBack?.Invoke(progressInfo);
                        fileContents.Clear();
                    }
                    // file transfer completed. (Block = 0)
                    else if (reaponseStream.Current.Block == 0)
                    {
                        // if file chunk exists, then write into stream.
                        if (fileContents.Any())
                        {
                            fileContents.OrderBy(c => c.Block).ToList().ForEach(c => c.Content.WriteTo(ms));
                            progressInfo.Message = $"File【{fileName}】current download progress【{ms.Length}】bytes.";
                            progressCallBack?.Invoke(progressInfo);
                            fileContents.Clear();
                        }

                        progressInfo.IsCompleted = true;
                        progressInfo.Message = $"File【{fileName}】download completed. SpentTime:{DateTime.Now - startTime}";
                        progressInfo.Result = fileName;
                        _logger.Information(progressInfo.Message);
                        progressCallBack?.Invoke(progressInfo);
                    }
                    else
                    {
                        // Add file chunk to list.
                        fileContents.Add(reaponseStream.Current);

                        // Collect file chunks then write into stream. (file chunk size decide by server code...)
                        if (fileContents.Count >= _settings.ChunkBufferCount)
                        {
                            fileContents.OrderBy(c => c.Block).ToList().ForEach(c => c.Content.WriteTo(ms));
                            progressInfo.Message = $"File【{fileName}】current download progress【{ms.Length}】bytes.";
                            progressCallBack?.Invoke(progressInfo);
                            fileContents.Clear();
                        }
                    }
                }

                return progressInfo.IsCompleted ? ms.GetReadOnlySequence().ToArray() : null;
            }
            catch (Exception ex)
            {
                _logger.Error($"File【{fileName}】download unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task MoveFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetFileClient();
            var progressInfo = new ProgressInfo();
            var sourceFileName = sourceFilePath;
            var destinationFileName = destFilePath;

            try
            {
                var request = new MoveFileRequest()
                {
                    SourceFileName = sourceFileName,
                    DestinationFileName = destinationFileName,
                    Overwrite = overwrite,
                    Mark = mark
                };

                progressInfo.Message = $"Currently move file from【{sourceFileName}】to【{destinationFileName}】...";
                progressCallBack?.Invoke(progressInfo);

                await client.MoveFileAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Move file from【{sourceFileName}】to【{destinationFileName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = destinationFileName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Move file from【{sourceFileName}】to【{destinationFileName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Move file from【{sourceFileName}】to【{destinationFileName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        public async Task CopyFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var mark = $"{Guid.NewGuid()}";
            var startTime = DateTime.Now;
            var (channel, client) = GetFileClient();
            var progressInfo = new ProgressInfo();
            var sourceFileName = sourceFilePath;
            var destinationFileName = destFilePath;

            try
            {
                var request = new CopyFileRequest()
                {
                    SourceFileName = sourceFileName,
                    DestinationFileName = destinationFileName,
                    Overwrite = overwrite,
                    Mark = mark
                };

                progressInfo.Message = $"Currently copy file from【{sourceFileName}】to【{destinationFileName}】...";
                progressCallBack?.Invoke(progressInfo);

                await client.CopyFileAsync(request, cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    progressInfo.IsCompleted = true;
                    progressInfo.Message = $"Copy file from【{sourceFileName}】to【{destinationFileName}】completed. SpentTime:{DateTime.Now - startTime}";
                    progressInfo.Result = destinationFileName;
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
                else
                {
                    progressInfo.IsCompleted = false;
                    progressInfo.Message = $"Copy file from【{sourceFileName}】to【{destinationFileName}】canceled. SpentTime:{DateTime.Now - startTime}";
                    _logger.Information(progressInfo.Message);
                    progressCallBack?.Invoke(progressInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Copy file from【{sourceFileName}】to【{destinationFileName}】unexpected exception happened.({ex.GetType()}):{ex.Message}");
                throw;
            }
            finally
            {
                // Shutdown the channel.
                await channel?.ShutdownAsync();
            }
        }

        #endregion

        #region Private Method

        #region gRPC manage related

        private (GrpcChannel, FileTransfer.FileTransferClient) GetFileClient()
        {
            var channel = GetGrpcChannel();
            var client = new FileTransfer.FileTransferClient(channel);
            return (channel, client);
        }

        private (GrpcChannel, DirectoryTransfer.DirectoryTransferClient) GetDirectoryClient()
        {
            var channel = GetGrpcChannel();
            var client = new DirectoryTransfer.DirectoryTransferClient(channel);
            return (channel, client);
        }

        private GrpcChannel GetGrpcChannel()
            => GrpcChannel.ForAddress(_settings.ServerAddress);

        #endregion

        #region Memory manage related

        private RecyclableMemoryStreamManager GetRecyclableMemoryStreamManager()
        {
            var blockSize = 1024;
            var largeBufferMultiple = 1024 * 1024; // 1 MB
            var maximumBufferSize = 16 * largeBufferMultiple; // 16 MB
            var maximumFreeSmallPoolBytes = 100 * blockSize;
            var maximumFreeLargePoolBytes = maximumBufferSize * 4;
            var recyclableMemoryStreamManager =
                new RecyclableMemoryStreamManager(blockSize, largeBufferMultiple, maximumBufferSize)
                {
                    AggressiveBufferReturn = true,
                    GenerateCallStacks = false,
                    MaximumFreeSmallPoolBytes = maximumFreeSmallPoolBytes,
                    MaximumFreeLargePoolBytes = maximumFreeLargePoolBytes,
                    ThrowExceptionOnToArray = true
                };
            recyclableMemoryStreamManager.StreamDisposed += RecyclableMemoryStreamManager_StreamDisposed;
            return recyclableMemoryStreamManager;
        }

        private void RecyclableMemoryStreamManager_StreamDisposed(object sender, RecyclableMemoryStreamManager.StreamDisposedEventArgs e)
            => _logger.Debug("File memory stream disposed.");

        #endregion

        #endregion
    }
}
