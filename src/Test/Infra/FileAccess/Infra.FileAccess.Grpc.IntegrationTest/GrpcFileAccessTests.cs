using System.Text;
using Infra.Core.FileAccess.Abstractions;
using NUnit.Framework;

namespace Infra.FileAccess.Grpc.IntegrationTest
{
    /// <summary>
    /// gRPC file access integration test cases
    /// !!!
    /// !!! Notice: Private gRPC server info, please don't commit into version control.
    /// !!!         There integration test cases for convenient test use.
    /// !!!
    /// </summary>
    public class GrpcFileAccessTests
    {
        private readonly IFileAccess _fileAccess;

        #region Properties

        private static string TempPath => "Temp";

        #endregion

        public GrpcFileAccessTests()
        {
            var startup = new Startup();

            _fileAccess = startup.GetService<IFileAccess>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [SetUp]
        public async Task SetUp() => await _fileAccess.CreateDirectoryAsync(TempPath);

        #region Sync

        #region Directory

        [Test]
        public void CreateDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.CreateDirectory(directoryPath));
        }

        [Test]
        public void DirectoryExistsNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void GetFilesNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFiles(directoryPath));
        }

        [Test]
        public void GetFilesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFiles(directoryPath, "*.txt"));
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories));
        }

        [Test]
        public void DeleteDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DeleteDirectory(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetSubDirectories(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetSubDirectories(directoryPath, "Another*"));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories));
        }

        [Test]
        public void DirectoryCompressNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DirectoryCompress(directoryPath, zipFilePath));
        }

        [Test]
        public void GetParentPathNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetParentPath(directoryPath));
        }

        [Test]
        public void GetCurrentDirectoryNameNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetCurrentDirectoryName(directoryPath));
        }

        #endregion

        [Test]
        public void FileExistsNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.FileExists(filePath));
        }

        [Test]
        public void SaveFileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.SaveFile(filePath, "資料種類"));
        }

        [Test]
        public void SaveFileWithEncodingNotSupported()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.SaveFile(filePath, "資料種類", Encoding.GetEncoding(950)));
        }

        [Test]
        public void DeleteFileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DeleteFile(filePath));
        }

        [Test]
        public void GetFileSizeNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFileSize(filePath));
        }

        [Test]
        public void ReadUtf8FileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.ReadTextFile(filePath));
        }

        [Test]
        public void ReadBig5FileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.Throws<NotSupportedException>(() => _fileAccess.ReadTextFile(filePath, encoding));
        }

        [Test]
        public void MoveFileNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Move", "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.MoveFile(sourceFilePath, destFilePath));
        }

        [Test]
        public void CopyFileNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Copy", "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.CopyFile(sourceFilePath, destFilePath));
        }

        #endregion

        #region Async

        #region Directory

        [Test]
        public async Task CreateDirectorySuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);

            Assert.IsTrue(await _fileAccess.DirectoryExistsAsync(directoryPath));
        }

        [Test]
        public async Task JudgeDirectoryNotExistsSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.IsFalse(await _fileAccess.DirectoryExistsAsync(directoryPath));
        }

        [Test]
        public async Task GetFilesSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            var files = await _fileAccess.GetFilesAsync(directoryPath);

            Assert.AreEqual(2, files.Length);
        }

        [Test]
        public async Task GetFilesWithSearchPatternSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            var files = await _fileAccess.GetFilesAsync(directoryPath, "*.txt");

            Assert.AreEqual(1, files.Length);
        }

        [Test]
        public async Task GetFilesWithSearchPatternAndOptionSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            await _fileAccess.CreateDirectoryAsync(subDirectoryPath);
            await _fileAccess.SaveFileAsync(Path.Combine(subDirectoryPath, "temp_3.txt"), "content_3");

            var files = await _fileAccess.GetFilesAsync(directoryPath, "*.txt", SearchOption.AllDirectories);

            Assert.AreEqual(2, files.Length);
        }

        [Test]
        public async Task DeleteDirectorySuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.DeleteDirectoryAsync(directoryPath);

            Assert.IsFalse(await _fileAccess.DirectoryExistsAsync(directoryPath));
        }

        [Test]
        public async Task GetSubDirectoriesSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            await _fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await _fileAccess.GetSubDirectoriesAsync(directoryPath);

            Assert.AreEqual(2, subdirectories.Length);
        }

        [Test]
        public async Task GetSubDirectoriesWithSearchPatternSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            await _fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await _fileAccess.GetSubDirectoriesAsync(directoryPath, "Another*");

            Assert.AreEqual(1, subdirectories.Length);
        }

        [Test]
        public async Task GetSubDirectoriesWithSearchPatternAndOptionSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var anotherDirectoryPath = Path.Combine(directoryPath, "AnotherCreatedDirectory");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.CreateDirectoryAsync(anotherDirectoryPath);
            await _fileAccess.CreateDirectoryAsync(Path.Combine(anotherDirectoryPath, "SubDirectory"));
            await _fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await _fileAccess.GetSubDirectoriesAsync(directoryPath, "Sub*", SearchOption.AllDirectories);

            Assert.AreEqual(2, subdirectories.Length);
        }

        [Test]
        public async Task DirectoryCompressSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await _fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            await _fileAccess.DirectoryCompressAsync(directoryPath, zipFilePath);

            Assert.IsTrue(await _fileAccess.FileExistsAsync(zipFilePath));
        }

        [Test]
        public void GetParentPathAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetParentPathAsync(directoryPath));
        }

        [Test]
        public void GetCurrentDirectoryNameAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetCurrentDirectoryNameAsync(directoryPath));
        }

        #endregion

        [Test]
        public async Task JudgeFileExistsSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "temp_1.txt");

            await _fileAccess.SaveFileAsync(filePath, "content_1");

            Assert.IsTrue(await _fileAccess.FileExistsAsync(filePath));
        }

        [Test]
        public async Task SaveFileSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            await _fileAccess.SaveFileAsync(filePath, "資料種類");

            Assert.IsTrue(await _fileAccess.FileExistsAsync(filePath));
        }

        [Test]
        public async Task SaveFileWithEncodingSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            await _fileAccess.SaveFileAsync(filePath, "資料種類", Encoding.GetEncoding(950));

            Assert.IsTrue(await _fileAccess.FileExistsAsync(filePath));
        }

        [Test]
        public async Task DeleteFileSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            await _fileAccess.SaveFileAsync(filePath, "資料種類");
            await _fileAccess.DeleteFileAsync(filePath);

            Assert.IsFalse(await _fileAccess.FileExistsAsync(filePath));
        }

        [Test]
        public async Task ReadUtf8FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "utf8.txt");

            await _fileAccess.SaveFileAsync(filePath, content);

            Assert.AreEqual(content, await _fileAccess.ReadTextFileAsync(filePath));
        }

        [Test]
        public async Task ReadBig5FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            await _fileAccess.SaveFileAsync(filePath, content, encoding);

            Assert.AreEqual(content, await _fileAccess.ReadTextFileAsync(filePath, encoding));
        }

        [Test]
        public async Task GetFileSizeSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "temp.txt");

            await _fileAccess.SaveFileAsync(filePath, "have_a_nice_day_!_1");

            var fileSize = await _fileAccess.GetFileSizeAsync(filePath);

            Assert.IsTrue(fileSize > 0);
        }

        [Test]
        public async Task MoveFileSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.CreateDirectoryAsync(subDirectoryPath);
            await _fileAccess.SaveFileAsync(filePath, "資料種類");
            await _fileAccess.MoveFileAsync(filePath, subFilePath);

            Assert.IsFalse(await _fileAccess.FileExistsAsync(filePath));
            Assert.IsTrue(await _fileAccess.FileExistsAsync(subFilePath));
        }

        [Test]
        public async Task CopyFileSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            await _fileAccess.CreateDirectoryAsync(directoryPath);
            await _fileAccess.CreateDirectoryAsync(subDirectoryPath);
            await _fileAccess.SaveFileAsync(filePath, "資料種類");
            await _fileAccess.CopyFileAsync(filePath, subFilePath);

            Assert.IsTrue(await _fileAccess.FileExistsAsync(filePath));
            Assert.IsTrue(await _fileAccess.FileExistsAsync(subFilePath));
        }

        #endregion

        [TearDown]
        public async Task TearDown() => await _fileAccess.DeleteDirectoryAsync(TempPath);
    }
}
