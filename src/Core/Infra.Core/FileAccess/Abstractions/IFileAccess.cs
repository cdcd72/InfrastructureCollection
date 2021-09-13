using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.FileAccess.Abstractions
{
    public interface IFileAccess
    {
        #region Sync Method

        #region Directory

        void CreateDirectory(string directoryPath);

        bool DirectoryExists(string directoryPath);

        string[] GetFiles(string directoryPath);

        string[] GetFiles(string directoryPath, string searchPattern);

        string[] GetFiles(string directoryPath, string searchPattern, SearchOption searchOption);

        void DeleteDirectory(string directoryPath);

        void DeleteDirectory(string directoryPath, bool recursive);

        string[] GetSubDirectories(string directoryPath);

        string[] GetSubDirectories(string directoryPath, string searchPattern);

        string[] GetSubDirectories(string directoryPath, string searchPattern, SearchOption searchOption);

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

        void MoveFile(string sourceFilePath, string destFilePath);

        void MoveFile(string sourceFilePath, string destFilePath, bool overwrite);

        void CopyFile(string sourceFilePath, string destFilePath);

        void CopyFile(string sourceFilePath, string destFilePath, bool overwrite);

        #endregion

        #region Async Method

        Task SaveFileAsync(string filePath, string content);

        Task SaveFileAsync(string filePath, string content, Encoding encoding);

        Task SaveFileAsync(string filePath, byte[] bytes);

        Task<string> ReadTextFileAsync(string filePath);

        Task<string> ReadTextFileAsync(string filePath, Encoding encoding);

        Task<byte[]> ReadFileAsync(string filePath);

        #endregion
    }
}