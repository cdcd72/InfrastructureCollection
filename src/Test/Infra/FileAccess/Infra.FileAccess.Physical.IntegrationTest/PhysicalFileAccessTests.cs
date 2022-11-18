using System.Reflection;
using System.Text;
using Infra.Core.FileAccess.Abstractions;
using NUnit.Framework;

namespace Infra.FileAccess.Physical.IntegrationTest
{
    public class PhysicalFileAccessTests
    {
        private readonly IFileAccess fileAccess;

        #region Properties

        private static string RootPath =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        private static string TempPath => Path.Combine(RootPath, "Temp");

        #endregion

        public PhysicalFileAccessTests()
        {
            var startup = new Startup();

            fileAccess = startup.GetService<IFileAccess>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [SetUp]
        public void SetUp() => fileAccess.CreateDirectory(TempPath);

        #region Sync

        #region Directory

        [Test]
        public void CreateDirectorySuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);

            Assert.That(fileAccess.DirectoryExists(directoryPath), Is.True);
        }

        [Test]
        public void JudgeDirectoryNotExistsSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.That(fileAccess.DirectoryExists(directoryPath), Is.False);
        }

        [Test]
        public void GetFilesSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            Assert.That(fileAccess.GetFiles(directoryPath), Has.Length.EqualTo(2));
        }

        [Test]
        public void GetFilesWithSearchPatternSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            Assert.That(fileAccess.GetFiles(directoryPath, "*.txt"), Has.Length.EqualTo(1));
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            fileAccess.CreateDirectory(subDirectoryPath);
            fileAccess.SaveFile(Path.Combine(subDirectoryPath, "temp_3.txt"), "content_3");

            Assert.That(fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories), Has.Length.EqualTo(2));
        }

        [Test]
        public void DeleteDirectorySuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.DeleteDirectory(directoryPath);

            Assert.That(fileAccess.DirectoryExists(directoryPath), Is.False);
        }

        [Test]
        public void GetSubDirectoriesSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.CreateDirectory(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            fileAccess.CreateDirectory(Path.Combine(directoryPath, "SubCreatedDirectory"));

            Assert.That(fileAccess.GetSubDirectories(directoryPath), Has.Length.EqualTo(2));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.CreateDirectory(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            fileAccess.CreateDirectory(Path.Combine(directoryPath, "SubCreatedDirectory"));

            Assert.That(fileAccess.GetSubDirectories(directoryPath, "Another*"), Has.Length.EqualTo(1));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var anotherDirectoryPath = Path.Combine(directoryPath, "AnotherCreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.CreateDirectory(anotherDirectoryPath);
            fileAccess.CreateDirectory(Path.Combine(anotherDirectoryPath, "SubDirectory"));
            fileAccess.CreateDirectory(Path.Combine(directoryPath, "SubCreatedDirectory"));

            Assert.That(fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories), Has.Length.EqualTo(2));
        }

        [Test]
        public void DirectoryCompressSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            fileAccess.DirectoryCompress(directoryPath, zipFilePath);

            Assert.That(fileAccess.FileExists(zipFilePath), Is.True);
        }

        [Test]
        public void GetParentPathSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            fileAccess.CreateDirectory(directoryPath);

            Assert.That(fileAccess.GetParentPath(directoryPath), Is.EqualTo(TempPath));
        }

        [Test]
        public void GetCurrentDirectoryNameSuccess()
        {
            const string directoryName = "CreatedDirectory";

            var directoryPath = Path.Combine(TempPath, directoryName);

            fileAccess.CreateDirectory(directoryPath);

            Assert.That(fileAccess.GetCurrentDirectoryName(directoryPath), Is.EqualTo(directoryName));
        }

        #endregion

        [Test]
        public void JudgeFileExistsSuccess()
        {
            var filePath = Path.Combine(TempPath, "temp_1.txt");

            fileAccess.SaveFile(filePath, "content_1");

            Assert.That(fileAccess.FileExists(filePath), Is.True);
        }

        [Test]
        public void SaveFileSuccess()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            fileAccess.SaveFile(filePath, "資料種類");

            Assert.That(fileAccess.FileExists(filePath), Is.True);
        }

        [Test]
        public void SaveFileWithEncodingSuccess()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            fileAccess.SaveFile(filePath, "資料種類", Encoding.GetEncoding(950));

            Assert.That(fileAccess.FileExists(filePath), Is.True);
        }

        [Test]
        public void DeleteFileSuccess()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            fileAccess.SaveFile(filePath, "資料種類");
            fileAccess.DeleteFile(filePath);

            Assert.That(fileAccess.FileExists(filePath), Is.False);
        }

        [Test]
        public void ReadUtf8FileSuccess()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "utf8.txt");

            fileAccess.SaveFile(filePath, content);

            Assert.That(fileAccess.ReadTextFile(filePath), Is.EqualTo(content));
        }

        [Test]
        public void ReadBig5FileSuccess()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            fileAccess.SaveFile(filePath, content, encoding);

            Assert.That(fileAccess.ReadTextFile(filePath, encoding), Is.EqualTo(content));
        }

        [Test]
        public void GetFileSizeSuccess()
        {
            var filePath = Path.Combine(TempPath, "temp.txt");

            fileAccess.SaveFile(filePath, "have_a_nice_day_!_1");

            Assert.That(fileAccess.GetFileSize(filePath), Is.GreaterThan(0));
        }

        [Test]
        public void MoveFileSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.CreateDirectory(subDirectoryPath);
            fileAccess.SaveFile(filePath, "資料種類");
            fileAccess.MoveFile(filePath, subFilePath);

            Assert.Multiple(() =>
            {
                Assert.That(fileAccess.FileExists(filePath), Is.False);
                Assert.That(fileAccess.FileExists(subFilePath), Is.True);
            });
        }

        [Test]
        public void CopyFileSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            fileAccess.CreateDirectory(directoryPath);
            fileAccess.CreateDirectory(subDirectoryPath);
            fileAccess.SaveFile(filePath, "資料種類");
            fileAccess.CopyFile(filePath, subFilePath);

            Assert.Multiple(() =>
            {
                Assert.That(fileAccess.FileExists(filePath), Is.True);
                Assert.That(fileAccess.FileExists(subFilePath), Is.True);
            });
        }

        #endregion

        #region Async

        #region Directory

        [Test]
        public void CreateDirectoryAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.CreateDirectoryAsync(directoryPath));
        }

        [Test]
        public void DirectoryExistsAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.DirectoryExistsAsync(directoryPath));
        }

        [Test]
        public void GetFilesAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetFilesAsync(directoryPath));
        }

        [Test]
        public void GetFilesWithSearchPatternAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetFilesAsync(directoryPath, "*.txt"));
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetFilesAsync(directoryPath, "*.txt", SearchOption.AllDirectories));
        }

        [Test]
        public void DeleteDirectoryAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.DeleteDirectoryAsync(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetSubDirectoriesAsync(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetSubDirectoriesAsync(directoryPath, "Another*"));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetSubDirectoriesAsync(directoryPath, "Sub*", SearchOption.AllDirectories));
        }

        [Test]
        public void DirectoryCompressAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.DirectoryCompressAsync(directoryPath, zipFilePath));
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
        public void FileExistsAsyncNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.FileExistsAsync(filePath));
        }

        [Test]
        public async Task SaveFileSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            await fileAccess.SaveFileAsync(filePath, "資料種類");

            Assert.That(fileAccess.FileExists(filePath), Is.True);
        }

        [Test]
        public async Task SaveFileWithEncodingSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            await fileAccess.SaveFileAsync(filePath, "資料種類", Encoding.GetEncoding(950));

            Assert.That(fileAccess.FileExists(filePath), Is.True);
        }

        [Test]
        public void DeleteFileAsyncNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.DeleteFileAsync(filePath));
        }

        [Test]
        public void GetFileSizeAsyncNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.GetFileSizeAsync(filePath));
        }

        [Test]
        public async Task ReadUtf8FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "utf8.txt");

            await fileAccess.SaveFileAsync(filePath, content);

            Assert.That(fileAccess.ReadTextFile(filePath), Is.EqualTo(content));
        }

        [Test]
        public async Task ReadBig5FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            await fileAccess.SaveFileAsync(filePath, content, encoding);

            Assert.That(fileAccess.ReadTextFile(filePath, encoding), Is.EqualTo(content));
        }

        [Test]
        public void MoveFileAsyncNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Move", "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.MoveFileAsync(sourceFilePath, destFilePath));
        }

        [Test]
        public void CopyFileAsyncNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Copy", "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => fileAccess.CopyFileAsync(sourceFilePath, destFilePath));
        }

        #endregion

        [TearDown]
        public void TearDown() => fileAccess.DeleteDirectory(TempPath);
    }
}
