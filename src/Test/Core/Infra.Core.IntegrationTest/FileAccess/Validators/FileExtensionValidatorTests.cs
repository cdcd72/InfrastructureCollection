using System.Reflection;
using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.FileAccess.Validators
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:識別項不應包含底線", Justification = "<暫止>")]
    public class FileExtensionValidatorTests
    {
        #region Properties

        private static string RootPath =>
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "TestData", "Files");

        #endregion

        [Test]
        public void Should_True_Validate_JPG() => Assert.That(IsValidFileExtensionByBytes("1.jpg"), Is.True);

        [Test]
        public void Should_True_Validate_FS_JPG() => Assert.That(IsValidFileExtensionByFileStream("1.jpg"), Is.True);

        [Test]
        public void Should_True_Validate_JPG_ffd8ffdb() => Assert.That(IsValidFileExtensionByBytes("37.jpg"), Is.True);

        [Test]
        public void Should_True_Validate_FS_JPG_ffd8ffdb() => Assert.That(IsValidFileExtensionByFileStream("37.jpg"), Is.True);

        [Test]
        public void Should_True_Validate_7Z() => Assert.That(IsValidFileExtensionByBytes("2.7z"), Is.True);

        [Test]
        public void Should_True_Validate_FS_7Z() => Assert.That(IsValidFileExtensionByFileStream("2.7z"), Is.True);


        [Test]
        public void Should_True_Validate_BMP() => Assert.That(IsValidFileExtensionByBytes("3.bmp"), Is.True);

        [Test]
        public void Should_True_Validate_FS_BMP() => Assert.That(IsValidFileExtensionByFileStream("3.bmp"), Is.True);

        [Test]
        public void Should_True_Validate_TIFF() => Assert.That(IsValidFileExtensionByBytes("4.tif"), Is.True);

        [Test]
        public void Should_True_Validate_FS_TIFF() => Assert.That(IsValidFileExtensionByFileStream("4.tif"), Is.True);

        [Test]
        public void Should_True_Validate_RTF() => Assert.That(IsValidFileExtensionByBytes("5.rtf"), Is.True);

        [Test]
        public void Should_True_Validate_FS_RTF() => Assert.That(IsValidFileExtensionByFileStream("5.rtf"), Is.True);


        [Test]
        public void Should_True_Validate_PPT() => Assert.That(IsValidFileExtensionByBytes("6.ppt"), Is.True);

        [Test]
        public void Should_True_Validate_FS_PPT() => Assert.That(IsValidFileExtensionByFileStream("6.ppt"), Is.True);


        [Test]
        public void Should_True_Validate_PPTX() => Assert.That(IsValidFileExtensionByBytes("7.pptx"), Is.True);

        [Test]
        public void Should_True_Validate_FS_PPTX() => Assert.That(IsValidFileExtensionByFileStream("7.pptx"), Is.True);

        [Test]
        public void Should_True_Validate_DOC() => Assert.That(IsValidFileExtensionByBytes("8.doc"), Is.True);

        [Test]
        public void Should_True_Validate_FS_DOC() => Assert.That(IsValidFileExtensionByFileStream("8.doc"), Is.True);

        [Test]
        public void Should_True_Validate_DOCX() => Assert.That(IsValidFileExtensionByBytes("9.docx"), Is.True);

        [Test]
        public void Should_True_Validate_FS_DOCX() => Assert.That(IsValidFileExtensionByFileStream("9.docx"), Is.True);

        [Test]
        public void Should_True_Validate_ODT() => Assert.That(IsValidFileExtensionByBytes("10.odt"), Is.True);

        [Test]
        public void Should_True_Validate_FS_ODT() => Assert.That(IsValidFileExtensionByFileStream("10.odt"), Is.True);

        [Test]
        public void Should_True_Validate_ODS() => Assert.That(IsValidFileExtensionByBytes("11.ods"), Is.True);

        [Test]
        public void Should_True_Validate_FS_ODS() => Assert.That(IsValidFileExtensionByFileStream("11.ods"), Is.True);

        [Test]
        public void Should_True_Validate_XLSX() => Assert.That(IsValidFileExtensionByBytes("12.xlsx"), Is.True);

        [Test]
        public void Should_True_Validate_FS_XLSX() => Assert.That(IsValidFileExtensionByFileStream("12.xlsx"), Is.True);

        [Test]
        public void Should_True_Validate_XLS() => Assert.That(IsValidFileExtensionByBytes("13.xls"), Is.True);

        [Test]
        public void Should_True_Validate_FS_XLS() => Assert.That(IsValidFileExtensionByFileStream("13.xls"), Is.True);

        [Test]
        public void Should_False_Validate_DAT() => Assert.That(IsValidFileExtensionByBytes("14.dat"), Is.False);

        [Test]
        public void Should_False_Validate_FS_DAT() => Assert.That(IsValidFileExtensionByFileStream("14.dat"), Is.False);

        [Test]
        public void Should_True_Validate_AVI() => Assert.That(IsValidFileExtensionByBytes("15.avi"), Is.True);

        [Test]
        public void Should_True_Validate_FS_AVI() => Assert.That(IsValidFileExtensionByFileStream("15.avi"), Is.True);

        [Test]
        public void Should_True_Validate_MOV() => Assert.That(IsValidFileExtensionByBytes("16.mov"), Is.True);

        [Test]
        public void Should_True_Validate_FS_MOV() => Assert.That(IsValidFileExtensionByFileStream("16.mov"), Is.True);


        [Test]
        public void Should_True_Validate_WMV() => Assert.That(IsValidFileExtensionByBytes("17.wmv"), Is.True);

        [Test]
        public void Should_True_Validate_FS_WMV() => Assert.That(IsValidFileExtensionByFileStream("17.wmv"), Is.True);

        [Test]
        public void Should_True_Validate_JPEG() => Assert.That(IsValidFileExtensionByBytes("18.jpeg"), Is.True);

        [Test]
        public void Should_True_Validate_FS_JPEG() => Assert.That(IsValidFileExtensionByFileStream("18.jpeg"), Is.True);

        [Test]
        public void Should_True_Validate_TXT() => Assert.That(IsValidFileExtensionByBytes("19.txt"), Is.True);

        [Test]
        public void Should_True_Validate_FS_TXT() => Assert.That(IsValidFileExtensionByFileStream("19.txt"), Is.True);

        [Test]
        public void Should_True_Validate_GIF() => Assert.That(IsValidFileExtensionByBytes("20.gif"), Is.True);

        [Test]
        public void Should_True_Validate_FS_GIF() => Assert.That(IsValidFileExtensionByFileStream("20.gif"), Is.True);

        [Test]
        public void Should_True_Validate_RM() => Assert.That(IsValidFileExtensionByBytes("21.rm"), Is.True);

        [Test]
        public void Should_True_Validate_FS_RM() => Assert.That(IsValidFileExtensionByFileStream("21.rm"), Is.True);

        [Test]
        public void Should_True_Validate_PNG() => Assert.That(IsValidFileExtensionByBytes("22.png"), Is.True);

        [Test]
        public void Should_True_Validate_FS_PNG() => Assert.That(IsValidFileExtensionByFileStream("22.png"), Is.True);

        [Test]
        public void Should_True_Validate_PDF() => Assert.That(IsValidFileExtensionByBytes("23.pdf"), Is.True);

        [Test]
        public void Should_True_Validate_FS_PDF() => Assert.That(IsValidFileExtensionByFileStream("23.pdf"), Is.True);

        [Test]
        public void Should_True_Validate_ODP() => Assert.That(IsValidFileExtensionByBytes("24.odp"), Is.True);

        [Test]
        public void Should_True_Validate_FS_ODP() => Assert.That(IsValidFileExtensionByFileStream("24.odp"), Is.True);

        [Test]
        public void Should_True_Validate_WAV() => Assert.That(IsValidFileExtensionByBytes("25.wav"), Is.True);

        [Test]
        public void Should_True_Validate_FS_WAV() => Assert.That(IsValidFileExtensionByFileStream("25.wav"), Is.True);

        [Test]
        public void Should_True_Validate_ODG() => Assert.That(IsValidFileExtensionByBytes("26.odg"), Is.True);

        [Test]
        public void Should_True_Validate_FS_ODG() => Assert.That(IsValidFileExtensionByFileStream("26.odg"), Is.True);

        [Test]
        public void Should_True_Validate_3GP() => Assert.That(IsValidFileExtensionByBytes("27.3gp"), Is.True);

        [Test]
        public void Should_True_Validate_FS_3GP() => Assert.That(IsValidFileExtensionByFileStream("27.3gp"), Is.True);

        [Test]
        public void Should_True_Validate_M4V() => Assert.That(IsValidFileExtensionByBytes("28.m4v"), Is.True);

        [Test]
        public void Should_True_Validate_FS_M4V() => Assert.That(IsValidFileExtensionByFileStream("28.m4v"), Is.True);

        [Test]
        public void Should_True_Validate_MKV() => Assert.That(IsValidFileExtensionByBytes("29.mkv"), Is.True);

        [Test]
        public void Should_True_Validate_FS_MKV() => Assert.That(IsValidFileExtensionByFileStream("29.mkv"), Is.True);

        [Test]
        public void Should_True_Validate_MP4() => Assert.That(IsValidFileExtensionByBytes("30.mp4"), Is.True);

        [Test]
        public void Should_True_Validate_FS_MP4() => Assert.That(IsValidFileExtensionByFileStream("30.mp4"), Is.True);

        [Test]
        public void Should_True_Validate_MPG() => Assert.That(IsValidFileExtensionByBytes("31.mpg"), Is.True);

        [Test]
        public void Should_True_Validate_FS_MPG() => Assert.That(IsValidFileExtensionByFileStream("31.mpg"), Is.True);

        [Test]
        public void Should_True_Validate_RAR() => Assert.That(IsValidFileExtensionByBytes("32.rar"), Is.True);

        [Test]
        public void Should_True_Validate_FS_RAR() => Assert.That(IsValidFileExtensionByFileStream("32.rar"), Is.True);

        [Test]
        public void Should_True_Validate_MP3() => Assert.That(IsValidFileExtensionByBytes("33.mp3"), Is.True);

        [Test]
        public void Should_True_Validate_FS_MP3() => Assert.That(IsValidFileExtensionByFileStream("33.mp3"), Is.True);

        [Test]
        public void Should_True_Validate_TIF() => Assert.That(IsValidFileExtensionByBytes("34.tif"), Is.True);

        [Test]
        public void Should_True_Validate_FS_TIF() => Assert.That(IsValidFileExtensionByFileStream("34.tif"), Is.True);

        [Test]
        public void Should_True_Validate_MPEG() => Assert.That(IsValidFileExtensionByBytes("35.mpeg"), Is.True);

        [Test]
        public void Should_True_Validate_FS_MPEG() => Assert.That(IsValidFileExtensionByFileStream("35.mpeg"), Is.True);

        [Test]
        public void Should_True_Validate_ZIP() => Assert.That(IsValidFileExtensionByBytes("36.zip"), Is.True);

        [Test]
        public void Should_True_Validate_FS_ZIP() => Assert.That(IsValidFileExtensionByFileStream("36.zip"), Is.True);

        [Test]
        public void Should_False_Validate_MSI() => Assert.That(IsValidFileExtensionByBytes("38.msi"), Is.False);

        [Test]
        public void Should_False_Validate_FS_MSI() => Assert.That(IsValidFileExtensionByFileStream("38.msi"), Is.False);

        [Test]
        public void Should_True_Validate_HTML() => Assert.That(IsValidFileExtensionByBytes("39.html"), Is.True);

        [Test]
        public void Should_True_Validate_FS_HTML() => Assert.That(IsValidFileExtensionByFileStream("39.html"), Is.True);

        #region Private Method

        private static bool IsValidFileExtensionByBytes(string fileName)
        {
            var filePath = Path.Combine(RootPath, fileName);
            var fileData = File.ReadAllBytes(filePath);
            return FileExtensionValidator.IsValidFileExtension(fileName, fileData, null);
        }

        private static bool IsValidFileExtensionByFileStream(string fileName)
        {
            var filePath = Path.Combine(RootPath, fileName);
            using var fs = File.OpenRead(filePath);
            return FileExtensionValidator.IsValidFileExtension(fileName, fs, null);
        }

        #endregion
    }
}
