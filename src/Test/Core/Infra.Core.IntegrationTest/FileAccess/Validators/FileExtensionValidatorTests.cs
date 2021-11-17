using System.IO;
using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.FileAccess.Validators
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:識別項不應包含底線", Justification = "<暫止>")]
    public class FileExtensionValidatorTests
    {
        private readonly string _folderPath;

        public FileExtensionValidatorTests() =>
            _folderPath =
                Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"TestData\Files");

        [Test]
        public void Should_True_Validate_JPG() => Assert.IsTrue(IsValidFileExtensionByBytes("1.jpg"));

        [Test]
        public void Should_True_Validate_FS_JPG() => Assert.IsTrue(IsValidFileExtensionByFileStream("1.jpg"));

        [Test]
        public void Should_True_Validate_JPG_ffd8ffdb() => Assert.IsTrue(IsValidFileExtensionByBytes("37.jpg"));

        [Test]
        public void Should_True_Validate_FS_JPG_ffd8ffdb() => Assert.IsTrue(IsValidFileExtensionByFileStream("37.jpg"));

        [Test]
        public void Should_True_Validate_7Z() => Assert.IsTrue(IsValidFileExtensionByBytes("2.7z"));

        [Test]
        public void Should_True_Validate_FS_7Z() => Assert.IsTrue(IsValidFileExtensionByFileStream("2.7z"));


        [Test]
        public void Should_True_Validate_BMP() => Assert.IsTrue(IsValidFileExtensionByBytes("3.bmp"));

        [Test]
        public void Should_True_Validate_FS_BMP() => Assert.IsTrue(IsValidFileExtensionByFileStream("3.bmp"));

        [Test]
        public void Should_True_Validate_TIFF() => Assert.IsTrue(IsValidFileExtensionByBytes("4.tif"));

        [Test]
        public void Should_True_Validate_FS_TIFF() => Assert.IsTrue(IsValidFileExtensionByFileStream("4.tif"));

        [Test]
        public void Should_True_Validate_RTF() => Assert.IsTrue(IsValidFileExtensionByBytes("5.rtf"));

        [Test]
        public void Should_True_Validate_FS_RTF() => Assert.IsTrue(IsValidFileExtensionByFileStream("5.rtf"));


        [Test]
        public void Should_True_Validate_PPT() => Assert.IsTrue(IsValidFileExtensionByBytes("6.ppt"));

        [Test]
        public void Should_True_Validate_FS_PPT() => Assert.IsTrue(IsValidFileExtensionByFileStream("6.ppt"));


        [Test]
        public void Should_True_Validate_PPTX() => Assert.IsTrue(IsValidFileExtensionByBytes("7.pptx"));

        [Test]
        public void Should_True_Validate_FS_PPTX() => Assert.IsTrue(IsValidFileExtensionByFileStream("7.pptx"));

        [Test]
        public void Should_True_Validate_DOC() => Assert.IsTrue(IsValidFileExtensionByBytes("8.doc"));

        [Test]
        public void Should_True_Validate_FS_DOC() => Assert.IsTrue(IsValidFileExtensionByFileStream("8.doc"));

        [Test]
        public void Should_True_Validate_DOCX() => Assert.IsTrue(IsValidFileExtensionByBytes("9.docx"));

        [Test]
        public void Should_True_Validate_FS_DOCX() => Assert.IsTrue(IsValidFileExtensionByFileStream("9.docx"));

        [Test]
        public void Should_True_Validate_ODT() => Assert.IsTrue(IsValidFileExtensionByBytes("10.odt"));

        [Test]
        public void Should_True_Validate_FS_ODT() => Assert.IsTrue(IsValidFileExtensionByFileStream("10.odt"));

        [Test]
        public void Should_True_Validate_ODS() => Assert.IsTrue(IsValidFileExtensionByBytes("11.ods"));

        [Test]
        public void Should_True_Validate_FS_ODS() => Assert.IsTrue(IsValidFileExtensionByFileStream("11.ods"));

        [Test]
        public void Should_True_Validate_XLSX() => Assert.IsTrue(IsValidFileExtensionByBytes("12.xlsx"));

        [Test]
        public void Should_True_Validate_FS_XLSX() => Assert.IsTrue(IsValidFileExtensionByFileStream("12.xlsx"));

        [Test]
        public void Should_True_Validate_XLS() => Assert.IsTrue(IsValidFileExtensionByBytes("13.xls"));

        [Test]
        public void Should_True_Validate_FS_XLS() => Assert.IsTrue(IsValidFileExtensionByFileStream("13.xls"));

        [Test]
        public void Should_False_Validate_DAT() => Assert.IsFalse(IsValidFileExtensionByBytes("14.dat"));

        [Test]
        public void Should_False_Validate_FS_DAT() => Assert.IsFalse(IsValidFileExtensionByFileStream("14.dat"));

        [Test]
        public void Should_True_Validate_AVI() => Assert.IsTrue(IsValidFileExtensionByBytes("15.avi"));

        [Test]
        public void Should_True_Validate_FS_AVI() => Assert.IsTrue(IsValidFileExtensionByFileStream("15.avi"));

        [Test]
        public void Should_True_Validate_MOV() => Assert.IsTrue(IsValidFileExtensionByBytes("16.mov"));

        [Test]
        public void Should_True_Validate_FS_MOV() => Assert.IsTrue(IsValidFileExtensionByFileStream("16.mov"));


        [Test]
        public void Should_True_Validate_WMV() => Assert.IsTrue(IsValidFileExtensionByBytes("17.wmv"));

        [Test]
        public void Should_True_Validate_FS_WMV() => Assert.IsTrue(IsValidFileExtensionByFileStream("17.wmv"));

        [Test]
        public void Should_True_Validate_JPEG() => Assert.IsTrue(IsValidFileExtensionByBytes("18.jpeg"));

        [Test]
        public void Should_True_Validate_FS_JPEG() => Assert.IsTrue(IsValidFileExtensionByFileStream("18.jpeg"));

        [Test]
        public void Should_True_Validate_TXT() => Assert.IsTrue(IsValidFileExtensionByBytes("19.txt"));

        [Test]
        public void Should_True_Validate_FS_TXT() => Assert.IsTrue(IsValidFileExtensionByFileStream("19.txt"));

        [Test]
        public void Should_True_Validate_GIF() => Assert.IsTrue(IsValidFileExtensionByBytes("20.gif"));

        [Test]
        public void Should_True_Validate_FS_GIF() => Assert.IsTrue(IsValidFileExtensionByFileStream("20.gif"));

        [Test]
        public void Should_True_Validate_RM() => Assert.IsTrue(IsValidFileExtensionByBytes("21.rm"));

        [Test]
        public void Should_True_Validate_FS_RM() => Assert.IsTrue(IsValidFileExtensionByFileStream("21.rm"));

        [Test]
        public void Should_True_Validate_PNG() => Assert.IsTrue(IsValidFileExtensionByBytes("22.png"));

        [Test]
        public void Should_True_Validate_FS_PNG() => Assert.IsTrue(IsValidFileExtensionByFileStream("22.png"));

        [Test]
        public void Should_True_Validate_PDF() => Assert.IsTrue(IsValidFileExtensionByBytes("23.pdf"));

        [Test]
        public void Should_True_Validate_FS_PDF() => Assert.IsTrue(IsValidFileExtensionByFileStream("23.pdf"));

        [Test]
        public void Should_True_Validate_ODP() => Assert.IsTrue(IsValidFileExtensionByBytes("24.odp"));

        [Test]
        public void Should_True_Validate_FS_ODP() => Assert.IsTrue(IsValidFileExtensionByFileStream("24.odp"));

        [Test]
        public void Should_True_Validate_WAV() => Assert.IsTrue(IsValidFileExtensionByBytes("25.wav"));

        [Test]
        public void Should_True_Validate_FS_WAV() => Assert.IsTrue(IsValidFileExtensionByFileStream("25.wav"));

        [Test]
        public void Should_True_Validate_ODG() => Assert.IsTrue(IsValidFileExtensionByBytes("26.odg"));

        [Test]
        public void Should_True_Validate_FS_ODG() => Assert.IsTrue(IsValidFileExtensionByFileStream("26.odg"));

        [Test]
        public void Should_True_Validate_3GP() => Assert.IsTrue(IsValidFileExtensionByBytes("27.3gp"));

        [Test]
        public void Should_True_Validate_FS_3GP() => Assert.IsTrue(IsValidFileExtensionByFileStream("27.3gp"));

        [Test]
        public void Should_True_Validate_M4V() => Assert.IsTrue(IsValidFileExtensionByBytes("28.m4v"));

        [Test]
        public void Should_True_Validate_FS_M4V() => Assert.IsTrue(IsValidFileExtensionByFileStream("28.m4v"));

        [Test]
        public void Should_True_Validate_MKV() => Assert.IsTrue(IsValidFileExtensionByBytes("29.mkv"));

        [Test]
        public void Should_True_Validate_FS_MKV() => Assert.IsTrue(IsValidFileExtensionByFileStream("29.mkv"));

        [Test]
        public void Should_True_Validate_MP4() => Assert.IsTrue(IsValidFileExtensionByBytes("30.mp4"));

        [Test]
        public void Should_True_Validate_FS_MP4() => Assert.IsTrue(IsValidFileExtensionByFileStream("30.mp4"));

        [Test]
        public void Should_True_Validate_MPG() => Assert.IsTrue(IsValidFileExtensionByBytes("31.mpg"));

        [Test]
        public void Should_True_Validate_FS_MPG() => Assert.IsTrue(IsValidFileExtensionByFileStream("31.mpg"));

        [Test]
        public void Should_True_Validate_RAR() => Assert.IsTrue(IsValidFileExtensionByBytes("32.rar"));

        [Test]
        public void Should_True_Validate_FS_RAR() => Assert.IsTrue(IsValidFileExtensionByFileStream("32.rar"));

        [Test]
        public void Should_True_Validate_MP3() => Assert.IsTrue(IsValidFileExtensionByBytes("33.mp3"));

        [Test]
        public void Should_True_Validate_FS_MP3() => Assert.IsTrue(IsValidFileExtensionByFileStream("33.mp3"));

        [Test]
        public void Should_True_Validate_TIF() => Assert.IsTrue(IsValidFileExtensionByBytes("34.tif"));

        [Test]
        public void Should_True_Validate_FS_TIF() => Assert.IsTrue(IsValidFileExtensionByFileStream("34.tif"));

        [Test]
        public void Should_True_Validate_MPEG() => Assert.IsTrue(IsValidFileExtensionByBytes("35.mpeg"));

        [Test]
        public void Should_True_Validate_FS_MPEG() => Assert.IsTrue(IsValidFileExtensionByFileStream("35.mpeg"));

        [Test]
        public void Should_True_Validate_ZIP() => Assert.IsTrue(IsValidFileExtensionByBytes("36.zip"));

        [Test]
        public void Should_True_Validate_FS_ZIP() => Assert.IsTrue(IsValidFileExtensionByFileStream("36.zip"));

        [Test]
        public void Should_False_Validate_MSI() => Assert.IsFalse(IsValidFileExtensionByBytes("38.msi"));

        [Test]
        public void Should_False_Validate_FS_MSI() => Assert.IsFalse(IsValidFileExtensionByFileStream("38.msi"));

        #region Private Method

        private bool IsValidFileExtensionByBytes(string fileName)
        {
            var filePath = Path.Combine(_folderPath, fileName);
            var fileData = File.ReadAllBytes(filePath);
            return FileExtensionValidator.IsValidFileExtension(fileName, fileData, null);
        }

        private bool IsValidFileExtensionByFileStream(string fileName)
        {
            var filePath = Path.Combine(_folderPath, fileName);
            using var fs = File.OpenRead(filePath);
            return FileExtensionValidator.IsValidFileExtension(fileName, fs, null);
        }

        #endregion
    }
}
