using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Infra.Core.FileAccess.Abstractions;
using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.FileAccess.Physical.IntegrationTest
{
    public class PhysicalFileAccessTests
    {
        private readonly string _rootPath;
        private readonly IFileAccess _fileAccess;
        private readonly string _filesPath;
        private readonly string _tempPath;
        private readonly string _nonUncPattern;

        public PhysicalFileAccessTests()
        {
            _rootPath =
                Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TestData");

            _fileAccess = new PhysicalFileAccess(_rootPath);

            // Can't operate directory
            _filesPath = Path.Combine(_rootPath, "Files");

            // Can operate directory
            _tempPath = Path.Combine(_rootPath, "Temp");

            _nonUncPattern = PathValidator.NonUncPattern;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

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
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);

            Assert.IsTrue(_fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void JudegeDirectoryExistsSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.IsTrue(_fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void GetFilesSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.AreEqual(2, _fileAccess.GetFiles(directoryPath).Length);
        }

        [Test]
        public void GetFilesWithSearchPatternSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.AreEqual(1, _fileAccess.GetFiles(directoryPath, "*.txt").Length);
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.AreEqual(4, _fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories).Length);
        }

        [Test]
        public void DeleteDirectorySuccess()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            _fileAccess.CreateDirectory(directoryPath);
            _fileAccess.DeleteDirectory(directoryPath);

            Assert.IsFalse(_fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.AreEqual(2, _fileAccess.GetSubDirectories(directoryPath).Length);
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.AreEqual(1, _fileAccess.GetSubDirectories(directoryPath, "Another*").Length);
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.AreEqual(2, _fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories).Length);
        }

        [Test]
        public void DirectoryCompressSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(_tempPath, "DirectoryCompress.zip");

            _fileAccess.DirectoryCompress(directoryPath, zipFilePath);

            Assert.IsTrue(_fileAccess.FileExists(zipFilePath));
        }

        [Test]
        public void GetParentPathSuccess()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.AreEqual(_nonUncPattern + _filesPath, _fileAccess.GetParentPath(directoryPath));
        }

        [Test]
        public void GetCurrentDirectoryNameSuccess()
        {
            var directoryName = "CreatedDirectory";
            var directoryPath = Path.Combine(_filesPath, directoryName);

            Assert.AreEqual(directoryName, _fileAccess.GetCurrentDirectoryName(directoryPath));
        }

        #endregion

        [Test]
        public void JudegeFileExistsSuccess()
        {
            var filePath = Path.Combine(_filesPath, "CreatedDirectory", "temp.txt");

            Assert.IsTrue(_fileAccess.FileExists(filePath));
        }

        [Test]
        public void SaveUtf8FileSuccess()
        {
            var filePath = Path.Combine(_tempPath, "utf8.txt");
            var expectedContent = "資料種類";

            // Save file
            _fileAccess.SaveFile(filePath, expectedContent);

            // Read file
            var actualContent = _fileAccess.ReadTextFile(filePath);

            Assert.AreEqual(expectedContent, actualContent);
        }

        [Test]
        public void SaveBig5FileSuccess()
        {
            var filePath = Path.Combine(_tempPath, "big5.txt");
            var expectedContent = "資料種類";
            var encoding = Encoding.GetEncoding(950);

            // Save file
            _fileAccess.SaveFile(filePath, expectedContent, encoding);

            // Read file
            var actualContent = _fileAccess.ReadTextFile(filePath, encoding);

            Assert.AreEqual(expectedContent, actualContent);
        }

        [Test]
        public void ReadUtf8FileSuccess()
        {
            var filePath = Path.Combine(_filesPath, "utf8.txt");

            Assert.AreEqual("資料種類", _fileAccess.ReadTextFile(filePath));
        }

        [Test]
        public void ReadBig5FileSuccess()
        {
            var filePath = Path.Combine(_filesPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.AreEqual("資料種類", _fileAccess.ReadTextFile(filePath, encoding));
        }

        [Test]
        public void GetFileSizeSuccess()
        {
            var filePath = Path.Combine(_filesPath, "CreatedDirectory", "temp.txt");

            Assert.IsTrue(_fileAccess.GetFileSize(filePath) > 0);
        }

        [Test]
        public void MoveFileSuccess()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");
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
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");
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
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.CreateDirectoryAsync(directoryPath));
        }

        [Test]
        public void DirectoryExistsAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DirectoryExistsAsync(directoryPath));
        }

        [Test]
        public void GetFilesAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFilesAsync(directoryPath));
        }

        [Test]
        public void GetFilesWithSearchPatternAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFilesAsync(directoryPath, "*.txt"));
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFilesAsync(directoryPath, "*.txt", SearchOption.AllDirectories));
        }

        [Test]
        public void DeleteDirectoryAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DeleteDirectoryAsync(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetSubDirectoriesAsync(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetSubDirectoriesAsync(directoryPath, "Another*"));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetSubDirectoriesAsync(directoryPath, "Sub*", SearchOption.AllDirectories));
        }

        [Test]
        public void DirectoryCompressAsyncNotSupported()
        {
            var directoryPath = Path.Combine(_filesPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(_tempPath, "DirectoryCompress.zip");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DirectoryCompressAsync(directoryPath, zipFilePath));
        }

        #endregion

        [Test]
        public void FileExistsAsyncNotSupported()
        {
            var filePath = Path.Combine(_filesPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.FileExistsAsync(filePath));
        }

        [Test]
        public async Task SaveUtf8FileSuccessAsync()
        {
            var filePath = Path.Combine(_tempPath, "utf8.txt");
            var expectedContent = "資料種類";

            // Save file
            await _fileAccess.SaveFileAsync(filePath, expectedContent);

            // Read file
            var actualContent = await _fileAccess.ReadTextFileAsync(filePath);

            Assert.AreEqual(expectedContent, actualContent);
        }

        [Test]
        public async Task SaveBig5FileSuccessAsync()
        {
            var filePath = Path.Combine(_tempPath, "big5.txt");
            var expectedContent = "資料種類";
            var encoding = Encoding.GetEncoding(950);

            // Save file
            await _fileAccess.SaveFileAsync(filePath, expectedContent, encoding);

            // Read file
            var actualContent = await _fileAccess.ReadTextFileAsync(filePath, encoding);

            Assert.AreEqual(expectedContent, actualContent);
        }

        [Test]
        public void DeleteFileAsyncNotSupported()
        {
            var filePath = Path.Combine(_filesPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.DeleteFileAsync(filePath));
        }

        [Test]
        public void GetFileSizeAsyncNotSupported()
        {
            var filePath = Path.Combine(_filesPath, "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.GetFileSizeAsync(filePath));
        }

        [Test]
        public async Task ReadUtf8FileSuccessAsync()
        {
            var filePath = Path.Combine(_filesPath, "utf8.txt");

            Assert.AreEqual("資料種類", await _fileAccess.ReadTextFileAsync(filePath));
        }

        [Test]
        public async Task ReadBig5FileSuccessAsync()
        {
            var filePath = Path.Combine(_filesPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.AreEqual("資料種類", await _fileAccess.ReadTextFileAsync(filePath, encoding));
        }

        [Test]
        public void MoveFileAsyncNotSupported()
        {
            var sourceFilePath = Path.Combine(_filesPath, "test.txt");
            var destFilePath = Path.Combine(_filesPath, "Move", "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.MoveFileAsync(sourceFilePath, destFilePath));
        }

        [Test]
        public void CopyFileAsyncNotSupported()
        {
            var sourceFilePath = Path.Combine(_filesPath, "test.txt");
            var destFilePath = Path.Combine(_filesPath, "Copy", "test.txt");

            Assert.ThrowsAsync<NotSupportedException>(() => _fileAccess.CopyFileAsync(sourceFilePath, destFilePath));
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            var directories = _fileAccess.GetSubDirectories(_tempPath);

            foreach (var directory in directories)
            {
                _fileAccess.DeleteDirectory(directory);
            }

            var files = _fileAccess.GetFiles(_tempPath);

            foreach (var file in files)
            {
                _fileAccess.DeleteFile(file);
            }
        }
    }
}
