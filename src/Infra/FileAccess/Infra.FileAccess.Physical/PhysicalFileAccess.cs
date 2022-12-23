using System.IO.Compression;
using System.Text;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Models;
using Infra.Core.FileAccess.Validators;
using Infra.FileAccess.Physical.Configuration;
using Infra.FileAccess.Physical.Configuration.Validators;
using Microsoft.Extensions.Options;

namespace Infra.FileAccess.Physical
{
    public class PhysicalFileAccess : IFileAccess
    {
        private readonly Settings settings;
        private readonly PathValidator pathValidator;

        public PhysicalFileAccess(IOptions<Settings> settings)
        {
            this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
            pathValidator = new PathValidator(this.settings.Roots);
        }

        #region Sync Method

        #region Directory

        public void CreateDirectory(string directoryPath)
            => Directory.CreateDirectory(GetVerifiedPath(directoryPath));

        public bool DirectoryExists(string directoryPath)
            => Directory.Exists(GetVerifiedPath(directoryPath));

        public string[] GetFiles(string directoryPath, string searchPattern = "", SearchOption searchOption = default)
            => Directory.GetFiles(GetVerifiedPath(directoryPath), searchPattern, searchOption);

        public void DeleteDirectory(string directoryPath, bool recursive = true)
            => Directory.Delete(GetVerifiedPath(directoryPath), recursive);

        public string[] GetSubDirectories(string directoryPath, string searchPattern = "", SearchOption searchOption = default)
            => Directory.GetDirectories(GetVerifiedPath(directoryPath), searchPattern, searchOption);

        public void DirectoryCompress(string directoryPath, string zipFilePath)
            => ZipFile.CreateFromDirectory(GetVerifiedPath(directoryPath), GetVerifiedPath(zipFilePath), CompressionLevel.Optimal, false);

        public string GetParentPath(string directoryPath)
            => Directory.GetParent(GetVerifiedPath(directoryPath))?.FullName;

        public string GetCurrentDirectoryName(string directoryPath)
            => new DirectoryInfo(GetVerifiedPath(directoryPath)).Name;

        #endregion

        public bool FileExists(string filePath)
            => File.Exists(GetVerifiedPath(filePath));

        public void SaveFile(string filePath, string content)
            => SaveFile(filePath, content, Encoding.UTF8);

        public void SaveFile(string filePath, string content, Encoding encoding)
            => SaveFile(filePath, encoding.GetBytes(content));

        public void SaveFile(string filePath, byte[] bytes)
            => File.WriteAllBytes(GetVerifiedPath(filePath), bytes);

        public void DeleteFile(string filePath)
            => File.Delete(GetVerifiedPath(filePath));

        public long GetFileSize(string filePath)
            => new FileInfo(GetVerifiedPath(filePath)).Length;

        public string ReadTextFile(string filePath)
            => ReadTextFile(filePath, Encoding.UTF8);

        public string ReadTextFile(string filePath, Encoding encoding)
        {
            var fileBytes = ReadFile(filePath);

            return encoding.GetString(fileBytes);
        }

        public byte[] ReadFile(string filePath)
            => File.ReadAllBytes(GetVerifiedPath(filePath));

        public void MoveFile(string sourceFilePath, string destFilePath, bool overwrite = true)
            => File.Move(GetVerifiedPath(sourceFilePath), GetVerifiedPath(destFilePath), overwrite);

        public void CopyFile(string sourceFilePath, string destFilePath, bool overwrite = true)
            => File.Copy(GetVerifiedPath(sourceFilePath), GetVerifiedPath(destFilePath), overwrite);

        public void AppendAllLines(string path, IEnumerable<string> contents)
            => AppendAllLines(path, contents, Encoding.UTF8);

        public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            path = GetVerifiedPath(path);

            File.AppendAllLines(path, contents, encoding);
        }

        public string[] ReadAllLines(string path)
            => ReadAllLines(path, Encoding.UTF8);

        public string[] ReadAllLines(string path, Encoding encoding)
        {
            path = GetVerifiedPath(path);

            return File.ReadAllLines(path, encoding);
        }

        public void AppendAllText(string path, string content)
            => AppendAllText(path, content, Encoding.UTF8);

        public void AppendAllText(string path, string content, Encoding encoding)
        {
            path = GetVerifiedPath(path);

            File.AppendAllText(path, content, encoding);
        }

        #endregion

        #region Async Method

        #region Directory

        public Task CreateDirectoryAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<bool> DirectoryExistsAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task DeleteDirectoryAsync(string directoryPath, bool recursive = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<string[]> GetSubDirectoriesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task DirectoryCompressAsync(string directoryPath, string zipFilePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<string> GetParentPathAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<string> GetCurrentDirectoryNameAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        #endregion

        public Task<bool> FileExistsAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public async Task SaveFileAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await SaveFileAsync(filePath, content, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task SaveFileAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await SaveFileAsync(filePath, encoding.GetBytes(content), progressCallBack, cancellationToken);

        public async Task SaveFileAsync(string filePath, byte[] bytes, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await File.WriteAllBytesAsync(GetVerifiedPath(filePath), bytes, cancellationToken);

        public Task DeleteFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<long> GetFileSizeAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public async Task<string> ReadTextFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await ReadTextFileAsync(filePath, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task<string> ReadTextFileAsync(string filePath, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var fileBytes = await ReadFileAsync(filePath, progressCallBack, cancellationToken);

            return encoding.GetString(fileBytes);
        }

        public async Task<byte[]> ReadFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await File.ReadAllBytesAsync(GetVerifiedPath(filePath), cancellationToken);

        public Task MoveFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task CopyFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public async Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await AppendAllLinesAsync(path, contents, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            path = GetVerifiedPath(path);

            await File.AppendAllLinesAsync(path, contents, encoding, cancellationToken);
        }

        public async Task<string[]> ReadAllLinesAsync(string path, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await ReadAllLinesAsync(path, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            path = GetVerifiedPath(path);

            return await File.ReadAllLinesAsync(path, encoding, cancellationToken);
        }

        public async Task AppendAllTextAsync(string path, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await AppendAllTextAsync(path, content, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task AppendAllTextAsync(string path, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            path = GetVerifiedPath(path);

            await File.AppendAllTextAsync(path, content, encoding, cancellationToken);
        }

        #endregion

        #region Private Method

        private string GetVerifiedPath(string path) => pathValidator.GetValidPath(path);

        #endregion
    }
}
