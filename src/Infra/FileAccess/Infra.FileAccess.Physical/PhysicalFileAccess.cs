using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Models;
using Infra.Core.FileAccess.Validators;

namespace Infra.FileAccess.Physical
{
    public class PhysicalFileAccess : IFileAccess
    {
        private readonly PathValidator _pathValidator;

        public PhysicalFileAccess(params string[] rootPaths)
        {
            if (rootPaths is null || (rootPaths is not null && (rootPaths.Length is 0 || rootPaths.Any(path => path is null or ""))))
                throw new ArgumentNullException(nameof(rootPaths));

            _pathValidator = new PathValidator(rootPaths);
        }

        #region Sync Method

        #region Directory

        public void CreateDirectory(string directoryPath)
            => Directory.CreateDirectory(GetVerifiedPath(directoryPath));

        public bool DirectoryExists(string directoryPath)
            => Directory.Exists(GetVerifiedPath(directoryPath));

        public string[] GetFiles(string directoryPath, string searchPattern = "", SearchOption searchOption = default)
            => Directory.GetFiles(GetVerifiedPath(directoryPath), searchPattern, searchOption);

        public void DeleteDirectory(string directoryPath)
            => DeleteDirectory(directoryPath, true);

        public void DeleteDirectory(string directoryPath, bool recursive)
            => Directory.Delete(GetVerifiedPath(directoryPath), recursive);

        public string[] GetSubDirectories(string directoryPath)
            => Directory.GetDirectories(GetVerifiedPath(directoryPath));

        public string[] GetSubDirectories(string directoryPath, string searchPattern)
            => Directory.GetDirectories(GetVerifiedPath(directoryPath), searchPattern);

        public string[] GetSubDirectories(string directoryPath, string searchPattern, SearchOption searchOption)
            => Directory.GetDirectories(GetVerifiedPath(directoryPath), searchPattern, searchOption);

        public void DirectoryCompress(string directoryPath, string zipFilePath)
            => ZipFile.CreateFromDirectory(GetVerifiedPath(directoryPath), GetVerifiedPath(zipFilePath), CompressionLevel.Optimal, false);

        public string GetParentPath(string directoryPath)
            => Directory.GetParent(GetVerifiedPath(directoryPath)).FullName;

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

        #endregion

        #region Async Method

        #region Directory

        public Task CreateDirectoryAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<bool> DirectoryExistsAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
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

        #endregion

        #region Private Method

        private string GetVerifiedPath(string path) => _pathValidator.GetValidPath(path);

        #endregion
    }
}
