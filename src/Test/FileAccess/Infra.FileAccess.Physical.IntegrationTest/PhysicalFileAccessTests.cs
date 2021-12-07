using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.FileAccess.Physical.IntegrationTest
{
    public class PhysicalFileAccessTests
    {
        private readonly IFileAccess _fileAccess;

        #region Properties

        private static string RootPath =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        private static string TempPath => Path.Combine(RootPath, "Temp");

        #endregion

        public PhysicalFileAccessTests()
        {
            _fileAccess = new PhysicalFileAccess(RootPath);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [SetUp]
        public void SetUp() => _fileAccess.CreateDirectory(TempPath);

        [Test]
        public void NewPhysicalFileAccessWithoutRoot()
        {
            Assert.Throws<ArgumentNullException>(() => new PhysicalFileAccess());
            Assert.Throws<ArgumentNullException>(() => new PhysicalFileAccess(null));
            Assert.Throws<ArgumentNullException>(() => new PhysicalFileAccess(new string[] { null }));
            Assert.Throws<ArgumentNullException>(() => new PhysicalFileAccess(""));
            Assert.Throws<ArgumentNullException>(() => new PhysicalFileAccess(new string[] { "" }));
        }

        #region Sync

        #region Directory

        [Test]
        public void CreateDirectorySuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);

            Assert.IsTrue(_fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void JudgeDirectoryNotExistsSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.IsFalse(_fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void GetFilesSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            Assert.AreEqual(2, _fileAccess.GetFiles(directoryPath).Length);
        }

        [Test]
        public void GetFilesWithSearchPatternSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");

            Assert.AreEqual(1, _fileAccess.GetFiles(directoryPath, "*.txt").Length);
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            _fileAccess.CreateDirectory(subDirectoryPath);
            _fileAccess.SaveFile(Path.Combine(subDirectoryPath, "temp_3.txt"), "content_3");

            Assert.AreEqual(2, _fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories).Length);
        }

        [Test]
        public void DeleteDirectorySuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.DeleteDirectory(directoryPath);

            Assert.IsFalse(_fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.CreateDirectory(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            _fileAccess.CreateDirectory(Path.Combine(directoryPath, "SubCreatedDirectory"));

            Assert.AreEqual(2, _fileAccess.GetSubDirectories(directoryPath).Length);
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.CreateDirectory(Path.Combine(directoryPath, "AnotherCreatedDirectory"));
            _fileAccess.CreateDirectory(Path.Combine(directoryPath, "SubCreatedDirectory"));

            Assert.AreEqual(1, _fileAccess.GetSubDirectories(directoryPath, "Another*").Length);
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var anotherDirectoryPath = Path.Combine(directoryPath, "AnotherCreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.CreateDirectory(anotherDirectoryPath);
            _fileAccess.CreateDirectory(Path.Combine(anotherDirectoryPath, "SubDirectory"));
            _fileAccess.CreateDirectory(Path.Combine(directoryPath, "SubCreatedDirectory"));

            Assert.AreEqual(2, _fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories).Length);
        }

        [Test]
        public void DirectoryCompressSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_1.txt"), "content_1");
            _fileAccess.SaveFile(Path.Combine(directoryPath, "temp_2.log"), "content_2");
            _fileAccess.DirectoryCompress(directoryPath, zipFilePath);

            Assert.IsTrue(_fileAccess.FileExists(zipFilePath));
        }

        [Test]
        public void GetParentPathSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);

            Assert.AreEqual(PathValidator.NonUncPattern + TempPath, _fileAccess.GetParentPath(directoryPath));
        }

        [Test]
        public void GetCurrentDirectoryNameSuccess()
        {
            const string directoryName = "CreatedDirectory";

            var directoryPath = Path.Combine(TempPath, directoryName);

            _fileAccess.CreateDirectory(directoryPath);

            Assert.AreEqual(directoryName, _fileAccess.GetCurrentDirectoryName(directoryPath));
        }

        #endregion

        [Test]
        public void JudgeFileExistsSuccess()
        {
            var filePath = Path.Combine(TempPath, "temp_1.txt");

            _fileAccess.SaveFile(filePath, "content_1");

            Assert.IsTrue(_fileAccess.FileExists(filePath));
        }

        [Test]
        public void SaveFileSuccess()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            _fileAccess.SaveFile(filePath, "資料種類");

            Assert.IsTrue(_fileAccess.FileExists(filePath));
        }

        [Test]
        public void SaveFileWithEncodingSuccess()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            _fileAccess.SaveFile(filePath, "資料種類", Encoding.GetEncoding(950));

            Assert.IsTrue(_fileAccess.FileExists(filePath));
        }

        [Test]
        public void DeleteFileSuccess()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            _fileAccess.SaveFile(filePath, "資料種類");
            _fileAccess.DeleteFile(filePath);

            Assert.IsFalse(_fileAccess.FileExists(filePath));
        }

        [Test]
        public void ReadUtf8FileSuccess()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "utf8.txt");

            _fileAccess.SaveFile(filePath, content);

            Assert.AreEqual(content, _fileAccess.ReadTextFile(filePath));
        }

        [Test]
        public void ReadBig5FileSuccess()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            _fileAccess.SaveFile(filePath, content, encoding);

            Assert.AreEqual(content, _fileAccess.ReadTextFile(filePath, encoding));
        }

        [Test]
        public void GetFileSizeSuccess()
        {
            var filePath = Path.Combine(TempPath, "temp.txt");

            _fileAccess.SaveFile(filePath, "have_a_nice_day_!_1");

            Assert.IsTrue(_fileAccess.GetFileSize(filePath) > 0);
        }

        [Test]
        public void MoveFileSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.CreateDirectory(subDirectoryPath);
            _fileAccess.SaveFile(filePath, "資料種類");
            _fileAccess.MoveFile(filePath, subFilePath);

            Assert.IsFalse(_fileAccess.FileExists(filePath));
            Assert.IsTrue(_fileAccess.FileExists(subFilePath));
        }

        [Test]
        public void CopyFileSuccess()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var subDirectoryPath = Path.Combine(directoryPath, "SubCreatedDirectory");
            var filePath = Path.Combine(directoryPath, "temp.txt");
            var subFilePath = Path.Combine(subDirectoryPath, "temp.txt");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.CreateDirectory(subDirectoryPath);
            _fileAccess.SaveFile(filePath, "資料種類");
            _fileAccess.CopyFile(filePath, subFilePath);

            Assert.IsTrue(_fileAccess.FileExists(filePath));
            Assert.IsTrue(_fileAccess.FileExists(subFilePath));
        }

        #endregion

        #region Async

        #region Directory

        [Test]
        public void CreateDirectoryAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.CreateDirectoryAsync(directoryPath));
        }

        [Test]
        public void DirectoryExistsAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DirectoryExistsAsync(directoryPath));
        }

        [Test]
        public void GetFilesAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFilesAsync(directoryPath));
        }

        [Test]
        public void GetFilesWithSearchPatternAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFilesAsync(directoryPath, "*.txt"));
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFilesAsync(directoryPath, "*.txt", SearchOption.AllDirectories));
        }

        [Test]
        public void DeleteDirectoryAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DeleteDirectoryAsync(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetSubDirectoriesAsync(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetSubDirectoriesAsync(directoryPath, "Another*"));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetSubDirectoriesAsync(directoryPath, "Sub*", SearchOption.AllDirectories));
        }

        [Test]
        public void DirectoryCompressAsyncNotSupported()
        {
            var directoryPath = Path.Combine(TempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(TempPath, "DirectoryCompress.zip");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DirectoryCompressAsync(directoryPath, zipFilePath));
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
        public void FileExistsAsyncNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.FileExistsAsync(filePath));
        }

        [Test]
        public async Task SaveFileSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "utf8.txt");

            await _fileAccess.SaveFileAsync(filePath, "資料種類");

            Assert.IsTrue(_fileAccess.FileExists(filePath));
        }

        [Test]
        public async Task SaveFileWithEncodingSuccessAsync()
        {
            var filePath = Path.Combine(TempPath, "big5.txt");

            await _fileAccess.SaveFileAsync(filePath, "資料種類", Encoding.GetEncoding(950));

            Assert.IsTrue(_fileAccess.FileExists(filePath));
        }

        [Test]
        public void DeleteFileAsyncNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DeleteFileAsync(filePath));
        }

        [Test]
        public void GetFileSizeAsyncNotSupported()
        {
            var filePath = Path.Combine(TempPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFileSizeAsync(filePath));
        }

        [Test]
        public async Task ReadUtf8FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "utf8.txt");

            await _fileAccess.SaveFileAsync(filePath, content);

            Assert.AreEqual(content, _fileAccess.ReadTextFile(filePath));
        }

        [Test]
        public async Task ReadBig5FileSuccessAsync()
        {
            const string content = "資料種類";

            var filePath = Path.Combine(TempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            await _fileAccess.SaveFileAsync(filePath, content, encoding);

            Assert.AreEqual(content, _fileAccess.ReadTextFile(filePath, encoding));
        }

        [Test]
        public void MoveFileAsyncNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Move", "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.MoveFileAsync(sourceFilePath, destFilePath));
        }

        [Test]
        public void CopyFileAsyncNotSupported()
        {
            var sourceFilePath = Path.Combine(TempPath, "test.txt");
            var destFilePath = Path.Combine(TempPath, "Copy", "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.CopyFileAsync(sourceFilePath, destFilePath));
        }

        #endregion

        [TearDown]
        public void TearDown() => _fileAccess.DeleteDirectory(TempPath);
    }
}
