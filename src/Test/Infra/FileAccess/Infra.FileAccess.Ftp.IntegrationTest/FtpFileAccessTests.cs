using System.Reflection;
using System.Text;
using Infra.Core.FileAccess.Abstractions;
using Infra.FileAccess.Ftp.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infra.FileAccess.Ftp.IntegrationTest
{
    /// <summary>
    /// Ftp file access integration test cases
    /// !!!
    /// !!! Notice: Private ftp server info, please don't commit into version control.
    /// !!!         There integration test cases for convenient test use.
    /// !!!
    /// </summary>
    [TestClass]
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

        [TestInitialize]
        public async Task SetUp()
        {
            if (!await fileAccess.DirectoryExistsAsync(FtpBasePath))
                await fileAccess.CreateDirectoryAsync(FtpBasePath);
        }

        #region Sync

        #region Directory

        [TestMethod]
        public void CreateDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.CreateDirectory(directoryPath));
        }

        [TestMethod]
        public void DirectoryExistsNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.DirectoryExists(directoryPath));
        }

        [TestMethod]
        public void GetFilesNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetFiles(directoryPath));
        }

        [TestMethod]
        public void GetFilesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetFiles(directoryPath, "*.txt"));
        }

        [TestMethod]
        public void GetFilesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories));
        }

        [TestMethod]
        public void DeleteDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.DeleteDirectory(directoryPath));
        }

        [TestMethod]
        public void GetSubDirectoriesNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath));
        }

        [TestMethod]
        public void GetSubDirectoriesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath, "Another*"));
        }

        [TestMethod]
        public void GetSubDirectoriesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories));
        }

        [TestMethod]
        public void DirectoryCompressNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
            var zipFtpPath = Path.Combine(FtpBasePath, "DirectoryCompress.zip");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.DirectoryCompress(directoryPath, zipFtpPath));
        }

        [TestMethod]
        public void GetParentPathNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetParentPath(directoryPath));
        }

        [TestMethod]
        public void GetCurrentDirectoryNameNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetCurrentDirectoryName(directoryPath));
        }

        #endregion

        [TestMethod]
        public void FileExistsNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "test.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.FileExists(ftpPath));
        }

        [TestMethod]
        public void SaveFileNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.SaveFile(ftpPath, "資料種類"));
        }

        [TestMethod]
        public void SaveFileWithEncodingNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.SaveFile(ftpPath, "資料種類", Encoding.GetEncoding(950)));
        }

        [TestMethod]
        public void SaveFileWithBytesNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "RMA_20190826.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.SaveFile(ftpPath, Array.Empty<byte>()));
        }

        [TestMethod]
        public void DeleteFileNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "test.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.DeleteFile(ftpPath));
        }

        [TestMethod]
        public void GetFileSizeNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "test.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.GetFileSize(ftpPath));
        }

        [TestMethod]
        public void ReadUtf8FileNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.ReadTextFile(ftpPath));
        }

        [TestMethod]
        public void ReadBig5FileNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.ReadTextFile(ftpPath, encoding));
        }

        [TestMethod]
        public void ReadFileNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.ReadFile(ftpPath));
        }

        [TestMethod]
        public void MoveFileNotSupported()
        {
            var sourceFtpPath = Path.Combine(FtpBasePath, "test.txt");
            var destFtpPath = Path.Combine(FtpBasePath, "Move", "test.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.MoveFile(sourceFtpPath, destFtpPath));
        }

        [TestMethod]
        public void CopyFileNotSupported()
        {
            var sourceFtpPath = Path.Combine(FtpBasePath, "test.txt");
            var destFtpPath = Path.Combine(FtpBasePath, "Copy", "test.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.CopyFile(sourceFtpPath, destFtpPath));
        }

        [TestMethod]
        public void AppendUtf8AllLinesNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.AppendAllLines(ftpPath, new[] {content}));
        }

        [TestMethod]
        public void AppendBig5AllLinesNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.AppendAllLines(ftpPath, new[] {content}, encoding));
        }

        [TestMethod]
        public void ReadUtf8AllLinesNotSupported()
        {
            Assert.ThrowsException<NotSupportedException>(() => fileAccess.ReadAllLines(Path.Combine(FtpBasePath, "utf8.txt")));
        }

        [TestMethod]
        public void ReadBig5AllLinesNotSupported()
        {
            var encoding = Encoding.GetEncoding(950);

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.ReadAllLines(Path.Combine(FtpBasePath, "big5.txt"), encoding));
        }

        [TestMethod]
        public void AppendUtf8AllTextNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.AppendAllText(ftpPath, content));
        }

        [TestMethod]
        public void AppendBig5AllTextNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.AppendAllText(ftpPath, content, encoding));
        }

        [TestMethod]
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

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.CompressFiles(files, zipFilePath));
        }

        [TestMethod]
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

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.CompressFiles(files, zipFilePath));
        }

        [TestMethod]
        public void CompressFilesAndReturnBytesWithFilePathNotSupported()
        {
            var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
            var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
            var files = new Dictionary<string, string>
            {
                { Path.GetFileName(ftpPath1), ftpPath1 },
                { Path.GetFileName(ftpPath2), ftpPath2 }
            };

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.CompressFiles(files));
        }

        [TestMethod]
        public void CompressFilesAndReturnBytesWithFileBytesNotSupported()
        {
            var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
            var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
            var files = new Dictionary<string, byte[]>
            {
                { Path.GetFileName(ftpPath1), Array.Empty<byte>() },
                { Path.GetFileName(ftpPath2), Array.Empty<byte>() }
            };

            Assert.ThrowsException<NotSupportedException>(() => fileAccess.CompressFiles(files));
        }

        #endregion

        #region Async

        #region Directory

        [TestMethod]
        public async Task CreateDirectorySuccessAsync()
        {
            var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(ftpPath);

            Assert.IsTrue(await fileAccess.DirectoryExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task JudgeDirectoryNotExistsSuccessAsync()
        {
            var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.IsFalse(await fileAccess.DirectoryExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task GetFilesSuccessAsync()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            var files = await fileAccess.GetFilesAsync(directoryPath);

            Assert.AreEqual(2, files.Length);
        }

        [TestMethod]
        public async Task GetFilesWithSearchPatternSuccessAsync()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            var files = await fileAccess.GetFilesAsync(directoryPath, @".txt");

            Assert.AreEqual(1, files.Length);
        }

        [TestMethod]
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

            Assert.AreEqual(2, files.Length);
        }

        [TestMethod]
        public async Task DeleteDirectorySuccessAsync()
        {
            var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(ftpPath);
            await fileAccess.DeleteDirectoryAsync(ftpPath);

            Assert.IsFalse(await fileAccess.DirectoryExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task GetSubDirectoriesSuccessAsync()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath);

            Assert.AreEqual(2, subdirectories.Length);
        }

        [TestMethod]
        public async Task GetSubDirectoriesWithSearchPatternSuccessAsync()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath, "Another*");

            Assert.AreEqual(1, subdirectories.Length);
        }

        [TestMethod]
        public async Task GetSubDirectoriesWithSearchPatternAndOptionSuccessAsync()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
            var anotherDirectoryPath = Path.Combine(directoryPath, "AnotherCreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(anotherDirectoryPath);
            await fileAccess.CreateDirectoryAsync(Path.Combine(anotherDirectoryPath, "SubDirectory"));
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath, "Sub*", SearchOption.AllDirectories);

            Assert.AreEqual(2, subdirectories.Length);
        }

        [TestMethod]
        public void DirectoryCompressAsyncNotSupported()
        {
            var directoryPath = Path.Combine(FtpBasePath, "CreatedDirectory");
            var zipFtpPath = Path.Combine(FtpBasePath, "DirectoryCompress.zip");

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.DirectoryCompressAsync(directoryPath, zipFtpPath));
        }

        [TestMethod]
        public void GetParentPathAsyncNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.GetParentPathAsync(ftpPath));
        }

        [TestMethod]
        public void GetCurrentDirectoryNameAsyncNotSupported()
        {
            var ftpPath = Path.Combine(FtpBasePath, "CreatedDirectory");

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.GetCurrentDirectoryNameAsync(ftpPath));
        }

        #endregion

        [TestMethod]
        public async Task JudgeFileExistsSuccessAsync()
        {
            var ftpPath = Path.Combine(FtpBasePath, "RMA_20190826.txt");

            Assert.IsFalse(await fileAccess.FileExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task SaveFileSuccessAsync()
        {
            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            await fileAccess.SaveFileAsync(ftpPath, "資料種類");

            Assert.IsTrue(await fileAccess.FileExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task SaveFileWithEncodingSuccessAsync()
        {
            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");

            await fileAccess.SaveFileAsync(ftpPath, "資料種類", Encoding.GetEncoding(950));

            Assert.IsTrue(await fileAccess.FileExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task SaveFileWithBytesSuccessAsync()
        {
            var ftpPath = await SaveFileAsync(FtpBasePath, "RMA_20190826.txt");

            Assert.IsTrue(await fileAccess.FileExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task DeleteFileSuccessAsync()
        {
            var ftpPath = await SaveFileAsync(FtpBasePath, "RMA_20190826.txt");

            await fileAccess.DeleteFileAsync(ftpPath);

            Assert.IsFalse(await fileAccess.FileExistsAsync(ftpPath));
        }

        [TestMethod]
        public async Task ReadUtf8FileSuccessAsync()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            await fileAccess.SaveFileAsync(ftpPath, content);

            Assert.AreEqual(content, await fileAccess.ReadTextFileAsync(ftpPath));
        }

        [TestMethod]
        public async Task ReadBig5FileSuccessAsync()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            await fileAccess.SaveFileAsync(ftpPath, content, encoding);

            Assert.AreEqual(content, await fileAccess.ReadTextFileAsync(ftpPath, encoding));
        }

        [TestMethod]
        public async Task GetFileSizeSuccessAsync()
        {
            var ftpPath = await SaveFileAsync(FtpBasePath, "RMA_20190826.txt");

            var fileSize = await fileAccess.GetFileSizeAsync(ftpPath);

            Assert.IsTrue(fileSize > 0);
        }

        [TestMethod]
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

            Assert.IsFalse(await fileAccess.FileExistsAsync(ftpPath));
            Assert.IsTrue(await fileAccess.FileExistsAsync(subFtpPath));
        }

        [TestMethod]
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

            Assert.IsTrue(await fileAccess.FileExistsAsync(ftpPath));
            Assert.IsTrue(await fileAccess.FileExistsAsync(subFtpPath));
        }

        [TestMethod]
        public void AppendUtf8AllLinesAsyncNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.AppendAllLinesAsync(ftpPath, new[] {content}));
        }

        [TestMethod]
        public void AppendBig5AllLinesAsyncNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.AppendAllLinesAsync(ftpPath, new[] {content}, encoding));
        }

        [TestMethod]
        public void ReadUtf8AllLinesAsyncNotSupported()
        {
            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.ReadAllLinesAsync(Path.Combine(FtpBasePath, "utf8.txt")));
        }

        [TestMethod]
        public void ReadBig5AllLinesAsyncNotSupported()
        {
            var encoding = Encoding.GetEncoding(950);

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.ReadAllLinesAsync(Path.Combine(FtpBasePath, "big5.txt"), encoding));
        }

        [TestMethod]
        public void AppendUtf8AllTextAsyncNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "utf8.txt");

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.AppendAllTextAsync(ftpPath, content));
        }

        [TestMethod]
        public void AppendBig5AllTextAsyncNotSupported()
        {
            const string content = "資料種類";

            var ftpPath = Path.Combine(FtpBasePath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.AppendAllTextAsync(ftpPath, content, encoding));
        }

        [TestMethod]
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

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files, zipFtpPath));
        }

        [TestMethod]
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

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files, zipFtpPath));
        }

        [TestMethod]
        public void CompressFilesAndReturnBytesWithFilePathAsyncNotSupported()
        {
            var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
            var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
            var files = new Dictionary<string, string>
            {
                { Path.GetFileName(ftpPath1), ftpPath1 },
                { Path.GetFileName(ftpPath2), ftpPath2 }
            };

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files));
        }

        [TestMethod]
        public void CompressFilesAndReturnBytesWithFileBytesAsyncNotSupported()
        {
            var ftpPath1 = Path.Combine(FtpBasePath, "utf8.txt");
            var ftpPath2 = Path.Combine(FtpBasePath, "utf8-2.txt");
            var files = new Dictionary<string, byte[]>
            {
                { Path.GetFileName(ftpPath1), Array.Empty<byte>() },
                { Path.GetFileName(ftpPath2), Array.Empty<byte>() }
            };

            Assert.ThrowsExceptionAsync<NotSupportedException>(() => fileAccess.CompressFilesAsync(files));
        }

        #endregion

        [TestCleanup]
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
}
