using System.Text;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Enums;
using Infra.Core.FileAccess.Models;
using Infra.Core.FileAccess.Validators;
using Infra.FileAccess.Physical.Configuration;
using Infra.FileAccess.Physical.Configuration.Validators;
using Ionic.Zip;
using Ionic.Zlib;
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

        using var zip = new ZipFile();

        zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
        zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");
        zip.AddDirectory(directoryPath);
        zip.Save(zipFilePath);
    }

    public void DirectorySplitCompress(string directoryPath, string zipFilePath, ZipDataUnit zipDataUnit, int segmentSize, int compressionLevel = 6)
    {
        directoryPath = GetVerifiedPath(directoryPath);
        zipFilePath = GetVerifiedPath(zipFilePath);

        using var zip = new ZipFile();

        zip.MaxOutputSegmentSize = segmentSize * (int)zipDataUnit;
        zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
        zip.BufferSize = 1024;
        zip.CaseSensitiveRetrieval = true;
        zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");
        zip.AddItem(directoryPath, string.Empty);
        zip.Save(zipFilePath);
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

        using (var zip = new ZipFile())
        {
            zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
            zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");

            foreach (var (compressName, path) in files)
            {
                var fileBytes = ReadFile(path);

                zip.AddEntry(compressName, fileBytes);
            }

            zip.Save(ms);
        }

        return ms.ToArray();
    }

    public byte[] CompressFiles(Dictionary<string, byte[]> files, int compressionLevel = 6)
    {
        using var ms = new MemoryStream();

        using (var zip = new ZipFile())
        {
            zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
            zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");

            foreach (var (compressName, fileBytes) in files)
            {
                zip.AddEntry(compressName, fileBytes);
            }

            zip.Save(ms);
        }

        return ms.ToArray();
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

        using var zip = new ZipFile();

        zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
        zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");
        zip.AddDirectory(directoryPath);
        zip.Save(zipFilePath);

        return Task.CompletedTask;
    }

    public Task DirectorySplitCompressAsync(string directoryPath, string zipFilePath, ZipDataUnit zipDataUnit, int segmentSize, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        directoryPath = GetVerifiedPath(directoryPath);
        zipFilePath = GetVerifiedPath(zipFilePath);

        using var zip = new ZipFile();

        zip.MaxOutputSegmentSize = segmentSize * (int)zipDataUnit;
        zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
        zip.BufferSize = 1024;
        zip.CaseSensitiveRetrieval = true;
        zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");
        zip.AddItem(directoryPath, string.Empty);
        zip.Save(zipFilePath);

        return Task.CompletedTask;
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

        using (var zip = new ZipFile())
        {
            zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
            zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");

            foreach (var (compressName, path) in files)
            {
                var fileBytes = await ReadFileAsync(path, progressCallBack, cancellationToken);

                zip.AddEntry(compressName, fileBytes);
            }

            zip.Save(ms);
        }

        return ms.ToArray();
    }

    public async Task<byte[]> CompressFilesAsync(Dictionary<string, byte[]> files, int compressionLevel = 6, Action<ProgressInfo> progressCallBack = null, CancellationToken cancellationToken = default)
    {
        await using var ms = new MemoryStream();

        using (var zip = new ZipFile())
        {
            zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
            zip.CompressionLevel = Enum.Parse<CompressionLevel>($"{compressionLevel}");

            foreach (var (compressName, fileBytes) in files)
            {
                zip.AddEntry(compressName, fileBytes);
            }

            zip.Save(ms);
        }

        return ms.ToArray();
    }

    #endregion

    #region Private Method

    private string GetVerifiedPath(string path) => pathValidator.GetValidPath(path);

    #endregion
}
