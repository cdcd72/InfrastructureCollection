using System.Text;
using Infra.Core.FileAccess.Models;

namespace Infra.Core.FileAccess.Abstractions
{
    public interface IFileAccess
    {
        #region Sync Method

        #region Directory

        void CreateDirectory(string directoryPath);

        bool DirectoryExists(string directoryPath);

        string[] GetFiles(string directoryPath, string searchPattern = "", SearchOption searchOption = default);

        void DeleteDirectory(string directoryPath, bool recursive = true);

        string[] GetSubDirectories(string directoryPath, string searchPattern = "", SearchOption searchOption = default);

        void DirectoryCompress(string directoryPath, string zipFilePath);

        string GetParentPath(string directoryPath);

        string GetCurrentDirectoryName(string directoryPath);

        #endregion

        bool FileExists(string filePath);

        void SaveFile(string filePath, string content);

        void SaveFile(string filePath, string content, Encoding encoding);

        void SaveFile(string filePath, byte[] bytes);

        void DeleteFile(string filePath);

        long GetFileSize(string filePath);

        string ReadTextFile(string filePath);

        string ReadTextFile(string filePath, Encoding encoding);

        byte[] ReadFile(string filePath);

        void MoveFile(string sourceFilePath, string destFilePath, bool overwrite = true);

        void CopyFile(string sourceFilePath, string destFilePath, bool overwrite = true);

        #endregion

        #region Async Method

        #region Directory

        Task CreateDirectoryAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<bool> DirectoryExistsAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task DeleteDirectoryAsync(string directoryPath, bool recursive = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<string[]> GetSubDirectoriesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task DirectoryCompressAsync(string directoryPath, string zipFilePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<string> GetParentPathAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<string> GetCurrentDirectoryNameAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        #endregion

        Task<bool> FileExistsAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task SaveFileAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task SaveFileAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task SaveFileAsync(string filePath, byte[] bytes, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task DeleteFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<long> GetFileSizeAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<string> ReadTextFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<string> ReadTextFileAsync(string filePath, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task<byte[]> ReadFileAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task MoveFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        Task CopyFileAsync(string sourceFilePath, string destFilePath, bool overwrite = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

        #endregion
    }
}
