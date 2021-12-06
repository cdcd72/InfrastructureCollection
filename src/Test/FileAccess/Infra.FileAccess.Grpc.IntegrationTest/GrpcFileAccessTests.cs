using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Infra.Core.FileAccess.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
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
        private readonly string _tempPath;

        public GrpcFileAccessTests()
        {
            _fileAccess = GetGrpcFileAccess();

            // Can operated directory...
            _tempPath = "Temp";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [SetUp]
        public async Task SetUp() => await _fileAccess.CreateDirectoryAsync(_tempPath);

        #region Sync

        #region Directory

        [Test]
        public void CreateDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.CreateDirectory(directoryPath));
        }

        [Test]
        public void DirectoryExistsNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DirectoryExists(directoryPath));
        }

        [Test]
        public void GetFilesNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFiles(directoryPath));
        }

        [Test]
        public void GetFilesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFiles(directoryPath, "*.txt"));
        }

        [Test]
        public void GetFilesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFiles(directoryPath, "*.txt", SearchOption.AllDirectories));
        }

        [Test]
        public void DeleteDirectoryNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DeleteDirectory(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetSubDirectories(directoryPath));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetSubDirectories(directoryPath, "Another*"));
        }

        [Test]
        public void GetSubDirectoriesWithSearchPatternAndOptionNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetSubDirectories(directoryPath, "Sub*", SearchOption.AllDirectories));
        }

        [Test]
        public void DirectoryCompressNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");
            var zipFilePath = Path.Combine(_tempPath, "DirectoryCompress.zip");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DirectoryCompress(directoryPath, zipFilePath));
        }

        [Test]
        public void GetParentPathNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetParentPath(directoryPath));
        }

        [Test]
        public void GetCurrentDirectoryNameNotSupported()
        {
            var directoryPath = Path.Combine(_tempPath, "CreatedDirectory");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetCurrentDirectoryName(directoryPath));
        }

        #endregion

        [Test]
        public void FileExistsNotSupported()
        {
            var filePath = Path.Combine(_tempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.FileExists(filePath));
        }

        [Test]
        public void SaveFileSuccess()
        {
            var filePath = Path.Combine(_tempPath, "utf8.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.SaveFile(filePath, "資料種類"));
        }

        [Test]
        public void SaveFileWithEncodingSuccess()
        {
            var filePath = Path.Combine(_tempPath, "big5.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.SaveFile(filePath, "資料種類", Encoding.GetEncoding(950)));
        }

        [Test]
        public void DeleteFileNotSupported()
        {
            var filePath = Path.Combine(_tempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.DeleteFile(filePath));
        }

        [Test]
        public void GetFileSizeNotSupported()
        {
            var filePath = Path.Combine(_tempPath, "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.GetFileSize(filePath));
        }

        [Test]
        public void ReadUtf8FileSuccess()
        {
            var filePath = Path.Combine(_tempPath, "utf8.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.ReadTextFile(filePath));
        }

        [Test]
        public void ReadBig5FileSuccess()
        {
            var filePath = Path.Combine(_tempPath, "big5.txt");
            var encoding = Encoding.GetEncoding(950);

            Assert.Throws<NotSupportedException>(() => _fileAccess.ReadTextFile(filePath, encoding));
        }

        [Test]
        public void MoveFileNotSupported()
        {
            var sourceFilePath = Path.Combine(_tempPath, "test.txt");
            var destFilePath = Path.Combine(_tempPath, "Move", "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.MoveFile(sourceFilePath, destFilePath));
        }

        [Test]
        public void CopyFileNotSupported()
        {
            var sourceFilePath = Path.Combine(_tempPath, "test.txt");
            var destFilePath = Path.Combine(_tempPath, "Copy", "test.txt");

            Assert.Throws<NotSupportedException>(() => _fileAccess.CopyFile(sourceFilePath, destFilePath));
        }

        #endregion

        #region Async

        #region Directory

        // TODO: Add test cases...

        #endregion

        #endregion

        [TearDown]
        public async Task TearDown() => await _fileAccess.DeleteDirectoryAsync(_tempPath);

        #region Private Method

        private static GrpcFileAccess GetGrpcFileAccess()
        {
            var mockLogger = new Mock<ILogger<GrpcFileAccess>>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockServerAddress = new Mock<IConfigurationSection>();
            mockServerAddress.Setup(m => m.Value)
                .Returns("https://localhost:5001");
            var mockChunkSize = new Mock<IConfigurationSection>();
            mockChunkSize.Setup(m => m.Value)
                .Returns("1048576");
            var mockChunkBufferCount = new Mock<IConfigurationSection>();
            mockChunkBufferCount.Setup(m => m.Value)
                .Returns("20");
            mockConfiguration.Setup(m => m.GetSection(It.Is<string>(key => key == "Grpc:File:ServerAddress")))
                .Returns(mockServerAddress.Object);
            mockConfiguration.Setup(m => m.GetSection(It.Is<string>(key => key == "Grpc:File:ChunkSize")))
                .Returns(mockChunkSize.Object);
            mockConfiguration.Setup(m => m.GetSection(It.Is<string>(key => key == "Grpc:File:ChunkBufferCount")))
                .Returns(mockChunkBufferCount.Object);

            return new GrpcFileAccess(mockLogger.Object, mockConfiguration.Object);
        }

        #endregion
    }
}
