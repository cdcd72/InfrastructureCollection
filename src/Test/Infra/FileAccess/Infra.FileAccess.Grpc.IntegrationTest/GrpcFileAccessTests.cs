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
        private readonly IFileAccess fileAccess;

        #region Properties

        private static string TempPath => "Temp";

        #endregion

        public GrpcFileAccessTests()
        {
            var startup = new Startup();

            fileAccess = startup.GetService<IFileAccess>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [SetUp]
        public async Task SetUp() => await fileAccess.CreateDirectoryAsync(TempPath);

        #region Sync

        #region Directory

        [Test]
        public void CreateDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.CreateDirectory(directoryPath));
        }

        [Test]
        public void DirectoryExistsNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void GetFilesNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetFiles(directoryPath));
        }

        [Test]
        public void GetFilesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetFiles(directoryPath, "*.txt"));
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories));
        }

        [Test]
        public void DeleteDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.DeleteDirectory(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath, "Another*"));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories));
        }

        [Test]
        public void DirectoryCompressNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            Assert.Throws<NotSupportedException>(() => fileAccess.DirectoryCompress(directoryPath, zipFilePath));
        }

        [Test]
        public void GetParentPathNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetParentPath(directoryPath));
        }

        [Test]
        public void GetCurrentDirectoryNameNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetCurrentDirectoryName(directoryPath));
        }

        #endregion

        [Test]
        public void FileExistsNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.FileExists(filePath));
        }

        [Test]
        public void SaveFileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.SaveFile(filePath, "資料種類"));
        }

        [Test]
        public void SaveFileWithEncodingNotSupported()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.SaveFile(filePath, "資料種類", Encoding.GetEncoding(950)));
        }

        [Test]
        public void DeleteFileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.DeleteFile(filePath));
        }

        [Test]
        public void GetFileSizeNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.GetFileSize(filePath));
        }

        [Test]
        public void ReadUtf8FileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.ReadTextFile(filePath));
        }

        [Test]
        public void ReadBig5FileNotSupported()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.Throws<NotSupportedException>(() => fileAccess.ReadTextFile(filePath, encoding));
        }

        [Test]
        public void MoveFileNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Move", "test.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.MoveFile(sourceFilePath, destFilePath));
        }

        [Test]
        public void CopyFileNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Copy", "test.txt");

            Assert.Throws<NotSupportedException>(() => fileAccess.CopyFile(sourceFilePath, destFilePath));
        }

        #endregion

        #region Async

        #region Directory

        [Test]
        public async Task CreateDirectorySuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);

            Assert.That(await fileAccess.DirectoryExistsAsync(directoryPath), Is.True);
        }

        [Test]
        public async Task JudgeDirectoryNotExistsSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.That(await fileAccess.DirectoryExistsAsync(directoryPath), Is.False);
        }

        [Test]
        public async Task GetFilesSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            var files = await fileAccess.GetFilesAsync(directoryPath);

            Assert.That(files, Has.Length.EqualTo(2));
        }

        [Test]
        public async Task GetFilesWithSearchPatternSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            var files = await fileAccess.GetFilesAsync(directoryPath, "*.txt");

            Assert.That(files, Has.Length.EqualTo(1));
        }

        [Test]
        public async Task GetFilesWithSearchPatternAndOptionSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            await fileAccess.CreateDirectoryAsync(subDirectoryPath);
            await fileAccess.SaveFileAsync(Path.Combine(subDirectoryPath, "temp_3.txt"), "content_3");

            var files = await fileAccess.GetFilesAsync(directoryPath, "*.txt", SearchOption.AllDirectories);

            Assert.That(files, Has.Length.EqualTo(2));
        }

        [Test]
        public async Task DeleteDirectorySuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.DeleteDirectoryAsync(directoryPath);

            Assert.That(await fileAccess.DirectoryExistsAsync(directoryPath), Is.False);
        }

        [Test]
        public async Task GetSubDirectoriesSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath);

            Assert.That(subdirectories, Has.Length.EqualTo(2));
        }

        [Test]
        public async Task GetSubDirectoriesWithSearchPatternSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath, "Another*");

            Assert.That(subdirectories, Has.Length.EqualTo(1));
        }

        [Test]
        public async Task GetSubDirectoriesWithSearchPatternAndOptionSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var anotherDirectoryPath = Path.Combine(directoryPath, "AnotherCreatedDirectory");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(anotherDirectoryPath);
            await fileAccess.CreateDirectoryAsync(Path.Combine(anotherDirectoryPath, "SubDirectory"));
            await fileAccess.CreateDirectoryAsync(Path.Combine(directoryPath, "SubCreatedDirectory"));

            var subdirectories = await fileAccess.GetSubDirectoriesAsync(directoryPath, "Sub*", SearchOption.AllDirectories);

            Assert.That(subdirectories, Has.Length.EqualTo(2));
        }

        [Test]
        public async Task DirectoryCompressSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            await fileAccess.SaveFileAsync(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            await fileAccess.DirectoryCompressAsync(directoryPath, zipFilePath);

            Assert.That(await fileAccess.FileExistsAsync(zipFilePath), Is.True);
        }

        [Test]
        public void GetParentPathAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetParentPathAsync(directoryPath));
        }

        [Test]
        public void GetCurrentDirectoryNameAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetCurrentDirectoryNameAsync(directoryPath));
        }

        #endregion

        [Test]
        public async Task JudgeFileExistsSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "temp_1.txt");

            await fileAccess.SaveFileAsync(filePath, "content_1");

            Assert.That(await fileAccess.FileExistsAsync(filePath), Is.True);
        }

        [Test]
        public async Task SaveFileSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            await fileAccess.SaveFileAsync(filePath, "資料種類");

            Assert.That(await fileAccess.FileExistsAsync(filePath), Is.True);
        }

        [Test]
        public async Task SaveFileWithEncodingSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            await fileAccess.SaveFileAsync(filePath, "資料種類", Encoding.GetEncoding(950));

            Assert.That(await fileAccess.FileExistsAsync(filePath), Is.True);
        }

        [Test]
        public async Task DeleteFileSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            await fileAccess.SaveFileAsync(filePath, "資料種類");
            await fileAccess.DeleteFileAsync(filePath);

            Assert.That(await fileAccess.FileExistsAsync(filePath), Is.False);
        }

        [Test]
        public async Task ReadUtf8FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "utf8.txt");

            await fileAccess.SaveFileAsync(filePath, content);

            Assert.That(await fileAccess.ReadTextFileAsync(filePath), Is.EqualTo(content));
        }

        [Test]
        public async Task ReadBig5FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            await fileAccess.SaveFileAsync(filePath, content, encoding);

            Assert.That(await fileAccess.ReadTextFileAsync(filePath, encoding), Is.EqualTo(content));
        }

        [Test]
        public async Task GetFileSizeSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "temp.txt");

            await fileAccess.SaveFileAsync(filePath, "have_a_nice_day_!_1");

            var fileSize = await fileAccess.GetFileSizeAsync(filePath);

            Assert.That(fileSize, Is.GreaterThan(0));
        }

        [Test]
        public async Task MoveFileSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(subDirectoryPath);
            await fileAccess.SaveFileAsync(filePath, "資料種類");
            await fileAccess.MoveFileAsync(filePath, subFilePath);

            Assert.Multiple(async () =>
            {
                Assert.That(await fileAccess.FileExistsAsync(filePath), Is.False);
                Assert.That(await fileAccess.FileExistsAsync(subFilePath), Is.True);
            });
        }

        [Test]
        public async Task CopyFileSuccessAsync()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            await fileAccess.CreateDirectoryAsync(directoryPath);
            await fileAccess.CreateDirectoryAsync(subDirectoryPath);
            await fileAccess.SaveFileAsync(filePath, "資料種類");
            await fileAccess.CopyFileAsync(filePath, subFilePath);

            Assert.Multiple(async () =>
            {
                Assert.That(await fileAccess.FileExistsAsync(filePath), Is.True);
                Assert.That(await fileAccess.FileExistsAsync(subFilePath), Is.True);
            });
        }

        #endregion

        [TearDown]
        public async Task TearDown() => await fileAccess.DeleteDirectoryAsync(TempPath);
    }
}
