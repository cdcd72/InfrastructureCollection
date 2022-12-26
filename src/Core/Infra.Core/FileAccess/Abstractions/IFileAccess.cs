using System.Text;
using Infra.Core.FileAccess.Models;

namespace Infra.Core.FileAccess.Abstractions;

public interface IFileAccess
{
    #region Sync Method

    #region Directory

    void CreateDirectory(string directoryPath);

    bool DirectoryExists(string directoryPath);

    string[] GetFiles(string directoryPath, string searchPattern = "", SearchOption searchOption = default);

    void DeleteDirectory(string directoryPath, bool recursive = true);

    string[] GetSubDirectories(string directoryPath, string searchPattern = "", SearchOption searchOption = default);

    void DirectoryCompress(string directoryPath, string zipFilePath, int compressionLevel = 6);

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

    void AppendAllLines(string path, IEnumerable<string> contents);

    void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding);

    string[] ReadAllLines(string path);

    string[] ReadAllLines(string path, Encoding encoding);

    void AppendAllText(string path, string content);

    void AppendAllText(string path, string content, Encoding encoding);

    void CompressFiles(Dictionary<string, string> files, string zipFilePath, int compressionLevel = 6);

    void CompressFiles(Dictionary<string, byte[]> files, string zipFilePath, int compressionLevel = 6);

    byte[] CompressFiles(Dictionary<string, string> files, int compressionLevel = 6);

    byte[] CompressFiles(Dictionary<string, byte[]> files, int compressionLevel = 6);

    #endregion

    #region Async Method

    #region Directory

    Task CreateDirectoryAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task<bool> DirectoryExistsAsync(string directoryPath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task DeleteDirectoryAsync(string directoryPath, bool recursive = true, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task<string[]> GetSubDirectoriesAsync(string directoryPath, string searchPattern = "", SearchOption searchOption = default, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task DirectoryCompressAsync(string directoryPath, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

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

    Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task<string[]> ReadAllLinesAsync(string path, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task AppendAllTextAsync(string path, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task AppendAllTextAsync(string path, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task CompressFilesAsync(Dictionary<string, string> files, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task CompressFilesAsync(Dictionary<string, byte[]> files, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task<byte[]> CompressFilesAsync(Dictionary<string, string> files, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    Task<byte[]> CompressFilesAsync(Dictionary<string, byte[]> files, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default);

    #endregion
}
