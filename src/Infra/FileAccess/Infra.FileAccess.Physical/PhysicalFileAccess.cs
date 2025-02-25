using System.IO.Compression;
using System.Text;
using Infra.Core.Extensions;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Enums;
using Infra.Core.FileAccess.Models;
using Infra.Core.FileAccess.Validators;
using Infra.FileAccess.Physical.Configuration;
using Infra.FileAccess.Physical.Configuration.Validators;
using Microsoft.Extensions.Options;

namespace Infra.FileAccess.Physical;

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

    public void DirectoryCompress(string directoryPath, string zipFilePath, int compressionLevel = 6)
    {
        directoryPath = GetVerifiedPath(directoryPath);
        zipFilePath = GetVerifiedPath(zipFilePath);

        ZipFile.CreateFromDirectory(directoryPath, zipFilePath, compressionLevel.ToCompressionLevel(), false);
    }

    public void DirectorySplitCompress(string directoryPath, string zipFilePath, ZipDataUnit zipDataUnit, int segmentSize, int compressionLevel = 6)
    {
        directoryPath = GetVerifiedPath(directoryPath);
        zipFilePath = GetVerifiedPath(zipFilePath);

        var tempZip = Path.Combine(Path.GetDirectoryName(zipFilePath), "temp.zip");

        ZipFile.CreateFromDirectory(directoryPath, tempZip);

        using (var zipToSplit = new FileStream(tempZip, FileMode.Open, System.IO.FileAccess.Read))
        {
            var buffer = new byte[8192];
            var partNumber = 1;
            var bytesRemaining = zipToSplit.Length;

            while (bytesRemaining > 0)
            {
                var partFileName = $"{zipFilePath}.{partNumber:D3}";
                using (var partFile = new FileStream(partFileName, FileMode.Create, System.IO.FileAccess.Write))
                {
                    var bytesToWrite = Math.Min(segmentSize * (int)zipDataUnit, bytesRemaining);
                    while (bytesToWrite > 0)
                    {
                        var bytesRead = zipToSplit.Read(buffer, 0, (int)Math.Min(buffer.Length, bytesToWrite));
                        partFile.Write(buffer, 0, bytesRead);
                        bytesToWrite -= bytesRead;
                        bytesRemaining -= bytesRead;
                    }
                }
                partNumber++;
            }
        }

        DeleteFile(tempZip);
    }

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

    public void AppendAllLines(string filePath, IEnumerable<string> contents)
        => AppendAllLines(filePath, contents, Encoding.UTF8);

    public void AppendAllLines(string filePath, IEnumerable<string> contents, Encoding encoding)
    {
        filePath = GetVerifiedPath(filePath);

        File.AppendAllLines(filePath, contents, encoding);
    }

    public string[] ReadAllLines(string filePath)
        => ReadAllLines(filePath, Encoding.UTF8);

    public string[] ReadAllLines(string filePath, Encoding encoding)
    {
        filePath = GetVerifiedPath(filePath);

        return File.ReadAllLines(filePath, encoding);
    }

    public void AppendAllText(string filePath, string content)
        => AppendAllText(filePath, content, Encoding.UTF8);

    public void AppendAllText(string filePath, string content, Encoding encoding)
    {
        filePath = GetVerifiedPath(filePath);

        File.AppendAllText(filePath, content, encoding);
    }

    public void CompressFiles(Dictionary<string, string> files, string zipFilePath, int compressionLevel = 6)
        => SaveFile(zipFilePath, CompressFiles(files, compressionLevel));

    public void CompressFiles(Dictionary<string, byte[]> files, string zipFilePath, int compressionLevel = 6)
        => SaveFile(zipFilePath, CompressFiles(files, compressionLevel));

    public byte[] CompressFiles(Dictionary<string, string> files, int compressionLevel = 6)
    {
        using var ms = new MemoryStream();

        using (var zip = new ZipArchive(ms, ZipArchiveMode.Update, true))
        {
            foreach (var (compressName, path) in files)
            {
                zip.CreateEntryFromFile(GetVerifiedPath(path), compressName, compressionLevel.ToCompressionLevel());
            }
        }

        byte[] compressedFileBytes;

        using (var binaryReader = new BinaryReader(ms))
        {
            ms.Position = 0;
            compressedFileBytes = binaryReader.ReadBytes((int)ms.Length);
        }

        return compressedFileBytes;
    }

    public byte[] CompressFiles(Dictionary<string, byte[]> files, int compressionLevel = 6)
    {
        using var ms = new MemoryStream();

        using (var zip = new ZipArchive(ms, ZipArchiveMode.Update, true))
        {
            foreach (var (compressName, fileBytes) in files)
            {
                var entry = zip.CreateEntry(compressName, compressionLevel.ToCompressionLevel());

                using var entryStream = entry.Open();

                entryStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        byte[] compressedFileBytes;

        using (var binaryReader = new BinaryReader(ms))
        {
            ms.Position = 0;
            compressedFileBytes = binaryReader.ReadBytes((int)ms.Length);
        }

        return compressedFileBytes;
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

    public Task DirectoryCompressAsync(string directoryPath, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        directoryPath = GetVerifiedPath(directoryPath);
        zipFilePath = GetVerifiedPath(zipFilePath);

        ZipFile.CreateFromDirectory(directoryPath, zipFilePath, compressionLevel.ToCompressionLevel(), false);

        return Task.CompletedTask;
    }

    public async Task DirectorySplitCompressAsync(string directoryPath, string zipFilePath, ZipDataUnit zipDataUnit, int segmentSize, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        directoryPath = GetVerifiedPath(directoryPath);
        zipFilePath = GetVerifiedPath(zipFilePath);

        var tempZip = Path.Combine(Path.GetDirectoryName(zipFilePath), "temp.zip");

        ZipFile.CreateFromDirectory(directoryPath, tempZip);

        await using (var zipToSplit = new FileStream(tempZip, FileMode.Open, System.IO.FileAccess.Read))
        {
            var buffer = new byte[8192];
            var partNumber = 1;
            var bytesRemaining = zipToSplit.Length;

            while (bytesRemaining > 0)
            {
                var partFileName = $"{zipFilePath}.{partNumber:D3}";
                await using (var partFile = new FileStream(partFileName, FileMode.Create, System.IO.FileAccess.Write))
                {
                    var bytesToWrite = Math.Min(segmentSize * (int)zipDataUnit, bytesRemaining);
                    while (bytesToWrite > 0)
                    {
                        var bytesRead = await zipToSplit.ReadAsync(buffer.AsMemory(0, (int)Math.Min(buffer.Length, bytesToWrite)), cancellationToken);
                        await partFile.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                        bytesToWrite -= bytesRead;
                        bytesRemaining -= bytesRead;
                    }
                }
                partNumber++;
            }
        }

        await DeleteFileAsync(tempZip, progressCallBack, cancellationToken);
    }

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

    public async Task AppendAllLinesAsync(string filePath, IEnumerable<string> contents, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        => await AppendAllLinesAsync(filePath, contents, Encoding.UTF8, progressCallBack, cancellationToken);

    public async Task AppendAllLinesAsync(string filePath, IEnumerable<string> contents, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        filePath = GetVerifiedPath(filePath);

        await File.AppendAllLinesAsync(filePath, contents, encoding, cancellationToken);
    }

    public async Task<string[]> ReadAllLinesAsync(string filePath, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        => await ReadAllLinesAsync(filePath, Encoding.UTF8, progressCallBack, cancellationToken);

    public async Task<string[]> ReadAllLinesAsync(string filePath, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        filePath = GetVerifiedPath(filePath);

        return await File.ReadAllLinesAsync(filePath, encoding, cancellationToken);
    }

    public async Task AppendAllTextAsync(string filePath, string content, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        => await AppendAllTextAsync(filePath, content, Encoding.UTF8, progressCallBack, cancellationToken);

    public async Task AppendAllTextAsync(string filePath, string content, Encoding encoding, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        filePath = GetVerifiedPath(filePath);

        await File.AppendAllTextAsync(filePath, content, encoding, cancellationToken);
    }

    public async Task CompressFilesAsync(Dictionary<string, string> files, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        => await SaveFileAsync(zipFilePath, await CompressFilesAsync(files, compressionLevel, progressCallBack, cancellationToken), progressCallBack, cancellationToken);

    public async Task CompressFilesAsync(Dictionary<string, byte[]> files, string zipFilePath, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
        => await SaveFileAsync(zipFilePath, await CompressFilesAsync(files, compressionLevel, progressCallBack, cancellationToken), progressCallBack, cancellationToken);

    public async Task<byte[]> CompressFilesAsync(Dictionary<string, string> files, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        await using var ms = new MemoryStream();

        using (var zip = new ZipArchive(ms, ZipArchiveMode.Update, true))
        {
            foreach (var (compressName, path) in files)
            {
                zip.CreateEntryFromFile(GetVerifiedPath(path), compressName, compressionLevel.ToCompressionLevel());
            }
        }

        byte[] compressedFileBytes;

        using (var binaryReader = new BinaryReader(ms))
        {
            ms.Position = 0;
            compressedFileBytes = binaryReader.ReadBytes((int)ms.Length);
        }

        return compressedFileBytes;
    }

    public async Task<byte[]> CompressFilesAsync(Dictionary<string, byte[]> files, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        await using var ms = new MemoryStream();

        using (var zip = new ZipArchive(ms, ZipArchiveMode.Update, true))
        {
            foreach (var (compressName, fileBytes) in files)
            {
                var entry = zip.CreateEntry(compressName, compressionLevel.ToCompressionLevel());

                await using var entryStream = entry.Open();

                await entryStream.WriteAsync(fileBytes, cancellationToken);
            }
        }

        byte[] compressedFileBytes;

        using (var binaryReader = new BinaryReader(ms))
        {
            ms.Position = 0;
            compressedFileBytes = binaryReader.ReadBytes((int)ms.Length);
        }

        return compressedFileBytes;
    }

    #endregion

    #region Private Method

    private string GetVerifiedPath(string path) => pathValidator.GetValidPath(path);

    #endregion
}
