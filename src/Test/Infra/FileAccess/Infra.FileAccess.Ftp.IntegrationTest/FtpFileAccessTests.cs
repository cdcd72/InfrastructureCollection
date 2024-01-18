using System.Reflection;
using System.Text;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Enums;
using NUnit.Framework;

namespace Infra.FileAccess.Ftp.IntegrationTest;

/// <summary>
/// Ftp file access integration test cases
/// !!!
/// !!! Notice: Private ftp server info, please don't commit into version control.
/// !!!         There integration test cases for convenient test use.
/// !!!
/// </summary>
public class FtpFileAccessTests
{
    private readonly IFileAccess fileAccess;

    #region Properties

    private static string RootPath =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    private static string InputPath => Path.Combine(RootPath, "TestData");

    private static string FtpBasePath => "upload";

    #endregion

    public FtpFileAccessTests()
    {
        var startup = new Startup();

        fileAccess = startup.GetService<IFileAccess>();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [SetUp]
    public async Task SetUp()
    {
        if (!await fileAccess.DirectoryExistsAsync(FtpBasePath))
            await fileAccess.CreateDirectoryAsync(FtpBasePath);
    }

    #region Sync

    #region Directory

    [Test]
    public void CreateDirectoryNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.CreateDirectory(directoryPath));
    }

    [Test]
    public void DirectoryExistsNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.DirectoryExists(directoryPath));
    }

    [Test]
    public void GetFilesNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetFiles(directoryPath));
    }

    [Test]
    public void GetFilesWithSearchPatternNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetFiles(directoryPath, "*.txt"));
    }

    [Test]
    public void GetFilesWithSearchPatternAndOptionNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories));
    }

    [Test]
    public void DeleteDirectoryNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.DeleteDirectory(directoryPath));
    }

    [Test]
    public void GetSubDirectoriesNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath));
    }

    [Test]
    public void GetSubDirectoriesWithSearchPatternNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath, "Another*"));
    }

    [Test]
    public void GetSubDirectoriesWithSearchPatternAndOptionNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories));
    }

    [Test]
    public void DirectoryCompressNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var zipFtpPath = Path.Combine(FtpBasePath, "DirectoryCompress.zip");

        Assert.Throws<NotSupportedException>(() => fileAccess.DirectoryCompress(directoryPath, zipFtpPath));
    }

    [Test]
    public void DirectorySplitCompressNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var zipFtpPath = Path.Combine(FtpBasePath, "DirectoryCompress.zip");

        Assert.Throws<NotSupportedException>(() => fileAccess.DirectorySplitCompress(directoryPath, zipFtpPath, ZipDataUnit.MB, 100));
    }

    [Test]
    public void GetParentPathNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetParentPath(directoryPath));
    }

    [Test]
    public void GetCurrentDirectoryNameNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetCurrentDirectoryName(directoryPath));
    }

    #endregion

    [Test]
    public void FileExistsNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "test.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.FileExists(ftpPath));
    }

    [Test]
    public void SaveFileNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.SaveFile(ftpPath, "資料種類"));
    }

    [Test]
    public void SaveFileWithEncodingNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.SaveFile(ftpPath, "資料種類", Encoding.GetEncoding(950)));
    }

    [Test]
    public void SaveFileWithBytesNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "RMA_20190826.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.SaveFile(ftpPath, Array.Empty<byte>()));
    }

    [Test]
    public void DeleteFileNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "test.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.DeleteFile(ftpPath));
    }

    [Test]
    public void GetFileSizeNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "test.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.GetFileSize(ftpPath));
    }

    [Test]
    public void ReadUtf8FileNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.ReadTextFile(ftpPath));
    }

    [Test]
    public void ReadBig5FileNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
        var encoding = Encoding.GetEncoding(950);

        Assert.Throws<NotSupportedException>(() => fileAccess.ReadTextFile(ftpPath, encoding));
    }

    [Test]
    public void ReadFileNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.ReadFile(ftpPath));
    }

    [Test]
    public void MoveFileNotSupported()
    {
        var sourceFtpPath = Path.Combine(FtpBasePath, "test.txt");
        var destFtpPath = Path.Combine(FtpBasePath, "Move", "test.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.MoveFile(sourceFtpPath, destFtpPath));
    }

    [Test]
    public void CopyFileNotSupported()
    {
        var sourceFtpPath = Path.Combine(FtpBasePath, "test.txt");
        var destFtpPath = Path.Combine(FtpBasePath, "Copy", "test.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.CopyFile(sourceFtpPath, destFtpPath));
    }

    [Test]
    public void AppendUtf8AllLinesNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.AppendAllLines(ftpPath, new[] {content}));
    }

    [Test]
    public void AppendBig5AllLinesNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
        var encoding = Encoding.GetEncoding(950);

        Assert.Throws<NotSupportedException>(() => fileAccess.AppendAllLines(ftpPath, new[] {content}, encoding));
    }

    [Test]
    public void ReadUtf8AllLinesNotSupported() => Assert.Throws<NotSupportedException>(() => fileAccess.ReadAllLines(Path.Combine(FtpBasePath, "utf8.txt")));

    [Test]
    public void ReadBig5AllLinesNotSupported()
    {
        var encoding = Encoding.GetEncoding(950);

        Assert.Throws<NotSupportedException>(() => fileAccess.ReadAllLines(Path.Combine(FtpBasePath, "big5.txt"), encoding));
    }

    [Test]
    public void AppendUtf8AllTextNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        Assert.Throws<NotSupportedException>(() => fileAccess.AppendAllText(ftpPath, content));
    }

    [Test]
    public void AppendBig5AllTextNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
        var encoding = Encoding.GetEncoding(950);

        Assert.Throws<NotSupportedException>(() => fileAccess.AppendAllText(ftpPath, content, encoding));
    }

    [Test]
    public void CompressFilesWithFilePathNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, string>
        {
            { Path.GetFileName(ftpPath1), ftpPath1 },
            { Path.GetFileName(ftpPath2), ftpPath2 }
        };
        var zipFilePath = Path.Combine(FtpBasePath, "compress.zip");

        Assert.Throws<NotSupportedException>(() => fileAccess.CompressFiles(files, zipFilePath));
    }

    [Test]
    public void CompressFilesWithFileBytesNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, byte[]>
        {
            { Path.GetFileName(ftpPath1), Array.Empty<byte>() },
            { Path.GetFileName(ftpPath2), Array.Empty<byte>() }
        };
        var zipFilePath = Path.Combine(FtpBasePath, "compress.zip");

        Assert.Throws<NotSupportedException>(() => fileAccess.CompressFiles(files, zipFilePath));
    }

    [Test]
    public void CompressFilesAndReturnBytesWithFilePathNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, string>
        {
            { Path.GetFileName(ftpPath1), ftpPath1 },
            { Path.GetFileName(ftpPath2), ftpPath2 }
        };

        Assert.Throws<NotSupportedException>(() => fileAccess.CompressFiles(files));
    }

    [Test]
    public void CompressFilesAndReturnBytesWithFileBytesNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, byte[]>
        {
            { Path.GetFileName(ftpPath1), Array.Empty<byte>() },
            { Path.GetFileName(ftpPath2), Array.Empty<byte>() }
        };

        Assert.Throws<NotSupportedException>(() => fileAccess.CompressFiles(files));
    }

    #endregion

    #region Async

    #region Directory

    [Test]
    public async Task CreateDirectorySuccessAsync()
    {
        var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        await fileAccess.CreateDirectoryAsync(ftpPath);

        Assert.That(await fileAccess.DirectoryExistsAsync(ftpPath), Is.True);
    }

    [Test]
    public async Task JudgeDirectoryNotExistsSuccessAsync()
    {
        var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.That(await fileAccess.DirectoryExistsAsync(ftpPath), Is.False);
    }

    [Test]
    public async Task GetFilesSuccessAsync()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
        await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

        var files = await fileAccess.GetFilesAsync(directoryPath);

        Assert.That(files, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task GetFilesWithSearchPatternSuccessAsync()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
        await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

        var files = await fileAccess.GetFilesAsync(directoryPath, @".txt");

        Assert.That(files, Has.Length.EqualTo(1));
    }

    [Test]
    public async Task GetFilesWithSearchPatternAndOptionSuccessAsync()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
        await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");
        await fileAccess.CreateDirectoryAsync(subDirectoryPath);
        await fileAccess.SaveFileAsync(Path.Combine(subDirectoryPath, "temp_3.txt"), "content_3");

        var files = await fileAccess.GetFilesAsync(directoryPath, ".txt", SearchOption.AllDirectories);

        Assert.That(files, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task DeleteDirectorySuccessAsync()
    {
        var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        await fileAccess.CreateDirectoryAsync(ftpPath);
        await fileAccess.DeleteDirectoryAsync(ftpPath);

        Assert.That(await fileAccess.DirectoryExistsAsync(ftpPath), Is.False);
    }

    [Test]
    public async Task GetSubDirectoriesSuccessAsync()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
        await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

        var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath);

        Assert.That(subdirectories, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task GetSubDirectoriesWithSearchPatternSuccessAsync()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
        await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

        var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath, "Another*");

        Assert.That(subdirectories, Has.Length.EqualTo(1));
    }

    [Test]
    public async Task GetSubDirectoriesWithSearchPatternAndOptionSuccessAsync()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var anotherDirectoryPath = Path.Combine(directoryPath, "AnotherCreatedDirectory");

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await fileAccess.CreateDirectoryAsync(anotherDirectoryPath);
        await fileAccess.CreateDirectoryAsync(Path.Combine(anotherDirectoryPath, "SubDirectory"));
        await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

        var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath, "Sub*", SearchOption.AllDirectories);

        Assert.That(subdirectories, Has.Length.EqualTo(2));
    }

    [Test]
    public void DirectoryCompressAsyncNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var zipFtpPath = Path.Combine(FtpBasePath, "DirectoryCompress.zip");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.DirectoryCompressAsync(directoryPath, zipFtpPath));
    }

    [Test]
    public void DirectorySplitCompressAsyncNotSupported()
    {
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var zipFtpPath = Path.Combine(FtpBasePath, "DirectoryCompress.zip");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.DirectorySplitCompressAsync(directoryPath, zipFtpPath, ZipDataUnit.MB, 100));
    }

    [Test]
    public void GetParentPathAsyncNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetParentPathAsync(ftpPath));
    }

    [Test]
    public void GetCurrentDirectoryNameAsyncNotSupported()
    {
        var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetCurrentDirectoryNameAsync(ftpPath));
    }

    #endregion

    [Test]
    public async Task JudgeFileExistsSuccessAsync()
    {
        var ftpPath = Path.Combine(FtpBasePath, "RMA_20190826.txt");

        Assert.That(await fileAccess.FileExistsAsync(ftpPath), Is.False);
    }

    [Test]
    public async Task SaveFileSuccessAsync()
    {
        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        await fileAccess.SaveFileAsync(ftpPath, "資料種類");

        Assert.That(await fileAccess.FileExistsAsync(ftpPath), Is.True);
    }

    [Test]
    public async Task SaveFileWithEncodingSuccessAsync()
    {
        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");

        await fileAccess.SaveFileAsync(ftpPath, "資料種類", Encoding.GetEncoding(950));

        Assert.That(await fileAccess.FileExistsAsync(ftpPath), Is.True);
    }

    [Test]
    public async Task SaveFileWithBytesSuccessAsync()
    {
        var ftpPath = await SaveFileAsync(FtpBasePath, "RMA_20190826.txt");

        Assert.That(await fileAccess.FileExistsAsync(ftpPath), Is.True);
    }

    [Test]
    public async Task DeleteFileSuccessAsync()
    {
        var ftpPath = await SaveFileAsync(FtpBasePath, "RMA_20190826.txt");

        await fileAccess.DeleteFileAsync(ftpPath);

        Assert.That(await fileAccess.FileExistsAsync(ftpPath), Is.False);
    }

    [Test]
    public async Task ReadUtf8FileSuccessAsync()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        await fileAccess.SaveFileAsync(ftpPath, content);

        Assert.That(await fileAccess.ReadTextFileAsync(ftpPath), Is.EqualTo(content));
    }

    [Test]
    public async Task ReadBig5FileSuccessAsync()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
        var encoding = Encoding.GetEncoding(950);

        await fileAccess.SaveFileAsync(ftpPath, content, encoding);

        Assert.That(await fileAccess.ReadTextFileAsync(ftpPath, encoding), Is.EqualTo(content));
    }

    [Test]
    public async Task GetFileSizeSuccessAsync()
    {
        var ftpPath = await SaveFileAsync(FtpBasePath, "RMA_20190826.txt");

        var fileSize = await fileAccess.GetFileSizeAsync(ftpPath);

        Assert.That(fileSize, Is.GreaterThan(0));
    }

    [Test]
    public async Task MoveFileSuccessAsync()
    {
        const string fileName = "RMA_20190826.txt";
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
        var ftpPath = Path.Combine(directoryPath, fileName);
        var subFtpPath = Path.Combine(subDirectoryPath, fileName);

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await SaveFileAsync(directoryPath, fileName);
        await fileAccess.CreateDirectoryAsync(subDirectoryPath);
        await fileAccess.MoveFileAsync(ftpPath, subFtpPath);

        Assert.Multiple(async () =>
        {
            Assert.That(await fileAccess.FileExistsAsync(ftpPath), Is.False);
            Assert.That(await fileAccess.FileExistsAsync(subFtpPath), Is.True);
        });
    }

    [Test]
    public async Task CopyFileSuccessAsync()
    {
        const string fileName = "RMA_20190826.txt";
        var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
        var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
        var ftpPath = Path.Combine(directoryPath, fileName);
        var subFtpPath = Path.Combine(subDirectoryPath, fileName);

        await fileAccess.CreateDirectoryAsync(directoryPath);
        await SaveFileAsync(directoryPath, fileName);
        await fileAccess.CreateDirectoryAsync(subDirectoryPath);
        await fileAccess.CopyFileAsync(ftpPath, subFtpPath);

        Assert.Multiple(async () =>
        {
            Assert.That(await fileAccess.FileExistsAsync(ftpPath), Is.True);
            Assert.That(await fileAccess.FileExistsAsync(subFtpPath), Is.True);
        });
    }

    [Test]
    public void AppendUtf8AllLinesAsyncNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.AppendAllLinesAsync(ftpPath, new[] {content}));
    }

    [Test]
    public void AppendBig5AllLinesAsyncNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
        var encoding = Encoding.GetEncoding(950);

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.AppendAllLinesAsync(ftpPath, new[] {content}, encoding));
    }

    [Test]
    public void ReadUtf8AllLinesAsyncNotSupported() => Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.ReadAllLinesAsync(Path.Combine(FtpBasePath, "utf8.txt")));

    [Test]
    public void ReadBig5AllLinesAsyncNotSupported()
    {
        var encoding = Encoding.GetEncoding(950);

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.ReadAllLinesAsync(Path.Combine(FtpBasePath, "big5.txt"), encoding));
    }

    [Test]
    public void AppendUtf8AllTextAsyncNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.AppendAllTextAsync(ftpPath, content));
    }

    [Test]
    public void AppendBig5AllTextAsyncNotSupported()
    {
        const string content = "資料種類";

        var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
        var encoding = Encoding.GetEncoding(950);

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.AppendAllTextAsync(ftpPath, content, encoding));
    }

    [Test]
    public void CompressFilesWithFilePathAsyncNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, string>
        {
            { Path.GetFileName(ftpPath1), ftpPath1 },
            { Path.GetFileName(ftpPath2), ftpPath2 }
        };
        var zipFtpPath = Path.Combine(FtpBasePath, "compress.zip");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files, zipFtpPath));
    }

    [Test]
    public void CompressFilesWithFileBytesAsyncNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, byte[]>
        {
            { Path.GetFileName(ftpPath1), Array.Empty<byte>() },
            { Path.GetFileName(ftpPath2), Array.Empty<byte>() }
        };
        var zipFtpPath = Path.Combine(FtpBasePath, "compress.zip");

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files, zipFtpPath));
    }

    [Test]
    public void CompressFilesAndReturnBytesWithFilePathAsyncNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, string>
        {
            { Path.GetFileName(ftpPath1), ftpPath1 },
            { Path.GetFileName(ftpPath2), ftpPath2 }
        };

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files));
    }

    [Test]
    public void CompressFilesAndReturnBytesWithFileBytesAsyncNotSupported()
    {
        var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
        var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
        var files = new Dictionary<string, byte[]>
        {
            { Path.GetFileName(ftpPath1), Array.Empty<byte>() },
            { Path.GetFileName(ftpPath2), Array.Empty<byte>() }
        };

        Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files));
    }

    #endregion

    [TearDown]
    public async Task TearDown()
    {
        if (await fileAccess.DirectoryExistsAsync(FtpBasePath))
            await fileAccess.DeleteDirectoryAsync(FtpBasePath);
    }

    #region Private Method

    private async Task<string> SaveFileAsync(string ftpBasePath, string fileName)
    {
        var fileBytes = await File.ReadAllBytesAsync(Path.Combine(InputPath, fileName));

        var ftpPath = Path.Combine(ftpBasePath, fileName);

        await fileAccess.SaveFileAsync(ftpPath, fileBytes);

        return ftpPath;
    }

    #endregion
}