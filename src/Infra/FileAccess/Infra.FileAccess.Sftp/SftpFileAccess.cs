using System.Text;
using System.Text.RegularExpressions;
using Infra.Core.Extensions;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Enums;
using Infra.Core.FileAccess.Models;
using Infra.FileAccess.Sftp.Configuration;
using Infra.FileAccess.Sftp.Configuration.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Infra.FileAccess.Sftp;

public class SftpFileAccess : IFileAccess
{
    private readonly ILogger<SftpFileAccess> logger;
    private readonly Settings settings;

    public SftpFileAccess(ILogger<SftpFileAccess> logger, IOptions<Settings> settings)
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

    public Task CreateDirectoryAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            sftp.CreateDirectory(GetRegularPath(directoryPath));
        }
        catch (Exception ex)
        {
            logger.Error($"Create sftp directory error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
        }

        return Task.CompletedTask;
    }

    public Task<bool> DirectoryExistsAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            return Task.FromResult(sftp.Exists(GetRegularPath(directoryPath)));
        }
        catch (Exception ex)
        {
            logger.Error($"Check sftp directory exists error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
        }
    }

    public Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        var sftpFiles = GetSftpFiles(directoryPath, searchPattern, searchOption);

        return Task.FromResult(sftpFiles.Where(sftpFile => !sftpFile.IsDirectory).Select(item => item.FullName).ToArray());
    }

    public Task DeleteDirectoryAsync(string directoryPath, bool recursive = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            directoryPath = GetRegularPath(directoryPath);

            foreach (var sftpFile in sftp.ListDirectory(directoryPath).Where(sftpFile => sftpFile.Name != "." && sftpFile.Name != ".."))
            {
                if (sftpFile.IsDirectory)
                    DeleteDirectoryAsync(sftpFile.FullName, recursive, progressCallBack, cancellationToken);
                else
                    sftp.DeleteFile(sftpFile.FullName);
            }

            sftp.DeleteDirectory(directoryPath);
        }
        catch (Exception ex)
        {
            logger.Error($"Delete sftp directory error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
        }

        return Task.CompletedTask;
    }

    public Task<string[]> GetSubDirectoriesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        var sftpFiles = GetSftpFiles(directoryPath, searchPattern, searchOption);

        return Task.FromResult(sftpFiles.Where(sftpFile => sftpFile.IsDirectory).Select(item => item.FullName).ToArray());
    }

    public Task DirectoryCompressAsync(string directoryPath, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task DirectorySplitCompressAsync(string directoryPath, string zipFilePath, ZipDataUnit zipDataUnit, int segmentSize, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<string> GetParentPathAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task<string> GetCurrentDirectoryNameAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    #endregion

    public Task<bool> FileExistsAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            return Task.FromResult(sftp.Exists(GetRegularPath(filePath)));
        }
        catch (Exception ex)
        {
            logger.Error($"Check sftp file exists error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
        }
    }

    public async Task SaveFileAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        => await SaveFileAsync(filePath, content, Encoding.UTF8, progressCallBack, cancellationToken);

    public async Task SaveFileAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        var contentBytes = encoding.GetBytes(content);

        await SaveFileAsync(filePath, contentBytes, progressCallBack, cancellationToken);
    }

    public Task SaveFileAsync(string filePath, byte[] bytes, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            sftp.UploadFile(new MemoryStream(bytes), GetRegularPath(filePath));
        }
        catch (Exception ex)
        {
            logger.Error($"Upload file to sftp error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
        }

        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            sftp.DeleteFile(GetRegularPath(filePath));
        }
        catch (Exception ex)
        {
            logger.Error($"Delete sftp file error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
        }

        return Task.CompletedTask;
    }

    public Task<long> GetFileSizeAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            return Task.FromResult(sftp.Get(GetRegularPath(filePath)).Attributes.Size);
        }
        catch (Exception ex)
        {
            logger.Error($"Get sftp file size error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
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

    public Task<byte[]> ReadFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            using var ms = new MemoryStream();

            sftp.DownloadFile(GetRegularPath(filePath), ms);

            return Task.FromResult(ms.ToArray());
        }
        catch (Exception ex)
        {
            logger.Error($"Download file from sftp error: {ex.Message}");

            throw;
        }
        finally
        {
            sftp.Disconnect();
        }
    }

    public async Task MoveFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await CopyFileAsync(sourceFilePath, destFilePath, overwrite, progressCallBack, cancellationToken);

            await DeleteFileAsync(sourceFilePath, progressCallBack, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Error($"Move sftp file error: {ex.Message}");

            throw;
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
            logger.Error($"Copy sftp file error: {ex.Message}");

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

    /// <summary>
    /// 取得 SFTP 清單項目
    /// </summary>
    /// <param name="path">路徑</param>
    /// <param name="searchPattern">搜尋 Pattern</param>
    /// <param name="searchOption">搜尋選項</param>
    /// <returns></returns>
    private IEnumerable<SftpFile> GetSftpFiles(string path, string searchPattern, SearchOption searchOption)
    {
        var sftpFiles = new List<SftpFile>();

        using var sftp = GetSftpClient();

        try
        {
            sftp.Connect();

            foreach (var sftpFile in sftp.ListDirectory(GetRegularPath(path)).Where(sftpFile => sftpFile.Name != "." && sftpFile.Name != ".."))
            {
                if (sftpFile.IsDirectory && searchOption is SearchOption.AllDirectories) sftpFiles.AddRange(GetSftpFiles(sftpFile.FullName, searchPattern, searchOption));

                sftpFiles.Add(sftpFile);
            }

            var regex = new Regex(searchPattern);

            return sftpFiles.Where(sftpFile => regex.IsMatch(sftpFile.Name));
        }
        finally
        {
            sftp.Disconnect();
        }
    }

    /// <summary>
    /// 取得 SftpClient
    /// </summary>
    /// <returns></returns>
    private SftpClient GetSftpClient() => new(settings.Host, settings.Port, settings.User, settings.Password);

    /// <summary>
    /// 取得常規的路徑
    /// </summary>
    /// <param name="path">路徑</param>
    /// <returns></returns>
    private static string GetRegularPath(string path) => path.Replace("\\", "/");

    #endregion
}