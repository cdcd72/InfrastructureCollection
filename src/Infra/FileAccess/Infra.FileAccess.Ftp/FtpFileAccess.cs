using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FluentFTP;
using Infra.Core.Extensions;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Enums;
using Infra.Core.FileAccess.Models;
using Infra.FileAccess.Ftp.Configuration;
using Infra.FileAccess.Ftp.Configuration.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infra.FileAccess.Ftp
{
    public class FtpFileAccess : IFileAccess
    {
        private readonly ILogger<FtpFileAccess> logger;
        private readonly Settings settings;

        public FtpFileAccess(ILogger<FtpFileAccess> logger, IOptions<Settings> settings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        }

        #region Sync Method

        #region Directory

        public void CreateDirectory(string directoryPath) => throw new NotSupportedException();

        public bool DirectoryExists(string directoryPath) => throw new NotSupportedException();

        public string[] GetFiles(string directoryPath, string searchPattern = "", SearchOption searchOption = default) => throw new NotSupportedException();

        public void DeleteDirectory(string directoryPath, bool recursive = true) => throw new NotSupportedException();

        public string[] GetSubDirectories(string directoryPath, string searchPattern = "", SearchOption searchOption = default) => throw new NotSupportedException();

        public void DirectoryCompress(string directoryPath, string zipFilePath, int compressionLevel = 6) => throw new NotSupportedException();

        public void DirectorySplitCompress(string directoryPath, string zipFilePath, ZipDataUnit zipDataUnit, int segmentSize, int compressionLevel = 6) => throw new NotSupportedException();

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

        public void AppendAllLines(string filePath, IEnumerable<string> contents) => throw new NotSupportedException();

        public void AppendAllLines(string filePath, IEnumerable<string> contents, Encoding encoding) => throw new NotSupportedException();

        public string[] ReadAllLines(string filePath) => throw new NotSupportedException();

        public string[] ReadAllLines(string filePath, Encoding encoding) => throw new NotSupportedException();

        public void AppendAllText(string filePath, string content) => throw new NotSupportedException();

        public void AppendAllText(string filePath, string content, Encoding encoding) => throw new NotSupportedException();

        public void CompressFiles(Dictionary<string, string> files, string zipFilePath, int compressionLevel = 6) => throw new NotSupportedException();

        public void CompressFiles(Dictionary<string, byte[]> files, string zipFilePath, int compressionLevel = 6) => throw new NotSupportedException();

        public byte[] CompressFiles(Dictionary<string, string> files, int compressionLevel = 6) => throw new NotSupportedException();

        public byte[] CompressFiles(Dictionary<string, byte[]> files, int compressionLevel = 6) => throw new NotSupportedException();

        #endregion

        #region Async Method

        #region Directory

        public async Task CreateDirectoryAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                await ftp.CreateDirectory(directoryPath, true, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Create ftp directory error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task<bool> DirectoryExistsAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                return await ftp.DirectoryExists(directoryPath, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Check ftp directory exists error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var ftpFiles = await GetFtpFilesAsync(directoryPath, searchPattern, searchOption, cancellationToken);

            return ftpFiles.Where(ftpFile => ftpFile.Type is FtpObjectType.File).Select(ftpFile => ftpFile.FullName).ToArray();
        }

        public async Task DeleteDirectoryAsync(string directoryPath, bool recursive = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                await ftp.DeleteDirectory(directoryPath, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Delete ftp directory error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task<string[]> GetSubDirectoriesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var ftpFiles = await GetFtpFilesAsync(directoryPath, searchPattern, searchOption, cancellationToken);

            return ftpFiles.Where(ftpFile => ftpFile.Type is FtpObjectType.Directory).Select(ftpFile => ftpFile.FullName).ToArray();
        }

        public Task DirectoryCompressAsync(string directoryPath, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task DirectorySplitCompressAsync(string directoryPath, string zipFilePath, ZipDataUnit zipDataUnit, int segmentSize, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<string> GetParentPathAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<string> GetCurrentDirectoryNameAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        #endregion

        public async Task<bool> FileExistsAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                return await ftp.FileExists(filePath, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Check ftp file exists error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task SaveFileAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await SaveFileAsync(filePath, content, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task SaveFileAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var contentBytes = encoding.GetBytes(content);

            await SaveFileAsync(filePath, contentBytes, progressCallBack, cancellationToken);
        }

        public async Task SaveFileAsync(string filePath, byte[] bytes, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                var startTime = DateTime.Now;

                var progress = new Progress<FtpProgress>(p =>
                {
                    var fileName = Path.GetFileName(filePath);

                    if (p.Progress is 100) {
                        progressCallBack?.Invoke(new ProgressInfo
                        {
                            IsCompleted = true,
                            Message = $"File【{fileName}】upload completed. SpentTime:{DateTime.Now - startTime}",
                            Result = fileName
                        });
                    } else {
                        progressCallBack?.Invoke(new ProgressInfo
                        {
                            IsCompleted = false,
                            Message = $"File【{fileName}】current upload progress {p.Progress} %."
                        });
                    }
                });

                await ftp.UploadBytes(bytes, filePath, FtpRemoteExists.Overwrite, false, progress, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Upload file to ftp error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task DeleteFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                await ftp.DeleteFile(filePath, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Delete ftp file error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task<long> GetFileSizeAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                return await ftp.GetFileSize(filePath, token: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Get ftp file size error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task<string> ReadTextFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await ReadTextFileAsync(filePath, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task<string> ReadTextFileAsync(string filePath, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var fileBytes = await ReadFileAsync(filePath, progressCallBack, cancellationToken);

            using var stream = new StreamReader(new MemoryStream(fileBytes), encoding);

            return await stream.ReadToEndAsync(cancellationToken);
        }

        public async Task<byte[]> ReadFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                var startTime = DateTime.Now;

                var progress = new Progress<FtpProgress>(p =>
                {
                    var fileName = Path.GetFileName(filePath);

                    if (p.Progress is 100) {
                        progressCallBack?.Invoke(new ProgressInfo
                        {
                            IsCompleted = true,
                            Message = $"File【{fileName}】download completed. SpentTime:{DateTime.Now - startTime}",
                            Result = fileName
                        });
                    } else {
                        progressCallBack?.Invoke(new ProgressInfo
                        {
                            IsCompleted = false,
                            Message = $"File【{fileName}】current download progress {p.Progress} %."
                        });
                    }
                });

                return await ftp.DownloadBytes(filePath, progress: progress, token: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Download file from ftp error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task MoveFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                await ftp.MoveFile(sourceFilePath, destFilePath, overwrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Move ftp file error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        public async Task CopyFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var bytes = await ReadFileAsync(sourceFilePath, progressCallBack, cancellationToken);

                await SaveFileAsync(destFilePath, bytes, progressCallBack, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error($"Copy ftp file error: {ex.Message}");

                throw;
            }
        }

        public Task AppendAllLinesAsync(string filePath, IEnumerable<string> contents, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task AppendAllLinesAsync(string filePath, IEnumerable<string> contents, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<string[]> ReadAllLinesAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<string[]> ReadAllLinesAsync(string filePath, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task AppendAllTextAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task AppendAllTextAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task CompressFilesAsync(Dictionary<string, string> files, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task CompressFilesAsync(Dictionary<string, byte[]> files, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<byte[]> CompressFilesAsync(Dictionary<string, string> files, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<byte[]> CompressFilesAsync(Dictionary<string, byte[]> files, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        #endregion

        #region Private Method

        private AsyncFtpClient GetAsyncFtpClient()
        {
            AsyncFtpClient ftp;

            if (!string.IsNullOrWhiteSpace(settings.User) && !string.IsNullOrWhiteSpace(settings.Password))
            {
                var credential = new NetworkCredential
                {
                    UserName = settings.User,
                    SecurePassword = settings.Password.ToSecureString()
                };

                ftp = new AsyncFtpClient(host: settings.Host, port: settings.Port, credentials: credential);
            }
            else
            {
                ftp = new AsyncFtpClient(settings.Host, settings.Port);
            }

            return ftp;
        }

        private async Task<FtpListItem[]> GetFtpFilesAsync(string path, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken)
        {
            using var ftp = GetAsyncFtpClient();

            try
            {
                await ftp.Connect(cancellationToken);

                var ftpListOption = searchOption switch
                {
                    SearchOption.TopDirectoryOnly => FtpListOption.Auto,
                    SearchOption.AllDirectories => FtpListOption.Recursive,
                    _ => FtpListOption.Auto
                };

                var ftpFiles = await ftp.GetListing(path, ftpListOption, cancellationToken);

                var regex = new Regex(searchPattern);

                return ftpFiles.Where(ftpFile => regex.IsMatch(ftpFile.Name)).ToArray();
            }
            catch (Exception ex)
            {
                logger.Error($"Get ftp files error: {ex.Message}");

                throw;
            }
            finally
            {
                await ftp.Disconnect(cancellationToken);
            }
        }

        #endregion
    }
}
