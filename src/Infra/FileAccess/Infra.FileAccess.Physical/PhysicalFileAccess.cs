using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Models;

namespace Infra.FileAccess.Physical
{
    public class PhysicalFileAccess : IFileAccess
    {
        #region Sync Method

        #region Directory

        public void CreateDirectory(string directoryPath)
            => Directory.CreateDirectory(directoryPath);

        public bool DirectoryExists(string directoryPath)
            => Directory.Exists(directoryPath);

        public string[] GetFiles(string directoryPath)
            => Directory.GetFiles(directoryPath);

        public string[] GetFiles(string directoryPath, string searchPattern)
            => Directory.GetFiles(directoryPath, searchPattern);

        public string[] GetFiles(string directoryPath, string searchPattern, SearchOption searchOption)
            => Directory.GetFiles(directoryPath, searchPattern, searchOption);

        public void DeleteDirectory(string directoryPath)
            => DeleteDirectory(directoryPath, true);

        public void DeleteDirectory(string directoryPath, bool recursive)
            => Directory.Delete(directoryPath, recursive);

        public string[] GetSubDirectories(string directoryPath)
            => Directory.GetDirectories(directoryPath);

        public string[] GetSubDirectories(string directoryPath, string searchPattern)
            => Directory.GetDirectories(directoryPath, searchPattern);

        public string[] GetSubDirectories(string directoryPath, string searchPattern, SearchOption searchOption)
            => Directory.GetDirectories(directoryPath, searchPattern, searchOption);

        public void DirectoryCompress(string directoryPath, string zipFilePath)
            => ZipFile.CreateFromDirectory(directoryPath, zipFilePath, CompressionLevel.Optimal, false);

        public string GetParentPath(string directoryPath)
            => Directory.GetParent(directoryPath).FullName;

        public string GetCurrentDirectoryName(string directoryPath)
            => new DirectoryInfo(directoryPath).Name;

        #endregion

        public bool FileExists(string filePath)
            => File.Exists(filePath);

        public void SaveFile(string filePath, string content)
            => SaveFile(filePath, content, Encoding.UTF8);

        public void SaveFile(string filePath, string content, Encoding encoding)
            => SaveFile(filePath, encoding.GetBytes(content));

        public void SaveFile(string filePath, byte[] bytes)
            => File.WriteAllBytes(filePath, bytes);

        public void DeleteFile(string filePath)
            => File.Delete(filePath);

        public long GetFileSize(string filePath)
            => new FileInfo(filePath).Length;

        public string ReadTextFile(string filePath)
            => ReadTextFile(filePath, Encoding.UTF8);

        public string ReadTextFile(string filePath, Encoding encoding)
        {
            var fileBytes = ReadFile(filePath);

            return encoding.GetString(fileBytes);
        }

        public byte[] ReadFile(string filePath)
            => File.ReadAllBytes(filePath);

        public void MoveFile(string sourceFilePath, string destFilePath)
            => MoveFile(sourceFilePath, destFilePath, true);

        public void MoveFile(string sourceFilePath, string destFilePath, bool overwrite)
            => File.Move(sourceFilePath, destFilePath, overwrite);

        public void CopyFile(string sourceFilePath, string destFilePath)
            => CopyFile(sourceFilePath, destFilePath, true);

        public void CopyFile(string sourceFilePath, string destFilePath, bool overwrite)
            => File.Copy(sourceFilePath, destFilePath, overwrite);

        #endregion

        #region Async Method

        public async Task SaveFileAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await SaveFileAsync(filePath, content, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task SaveFileAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await SaveFileAsync(filePath, encoding.GetBytes(content), progressCallBack, cancellationToken);

        public async Task SaveFileAsync(string filePath, byte[] bytes, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

        public async Task<string> ReadTextFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await ReadTextFileAsync(filePath, Encoding.UTF8, progressCallBack, cancellationToken);

        public async Task<string> ReadTextFileAsync(string filePath, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        {
            var fileBytes = await ReadFileAsync(filePath, progressCallBack, cancellationToken);

            return encoding.GetString(fileBytes);
        }

        public async Task<byte[]> ReadFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
            => await File.ReadAllBytesAsync(filePath, cancellationToken);

        #endregion
    }
}
