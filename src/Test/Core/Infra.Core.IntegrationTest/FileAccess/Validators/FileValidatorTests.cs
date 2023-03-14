using System.Reflection;
using Infra.Core.FileAccess.Validators;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.FileAccess.Validators;

public class FileValidatorTests
{
    #region Properties

    private static string RootPath =>
        Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "TestData", "Files");

    #endregion

    [Test]
    public void FileExtensionsNotAssigned() => Assert.Throws<ArgumentException>(() => FileValidator.IsValidFileExtensions("1.txt", null));

    [Test]
    public void InvalidFileExtensions() => Assert.That(FileValidator.IsValidFileExtensions("1.txt", new[] { "jpg" }), Is.False);

    [Test]
    public void ValidFileExtensions() => Assert.That(FileValidator.IsValidFileExtensions("1.jpg", new[] { "jpg" }), Is.True);

    [Test]
    public void ValidFileExtensionsWithUpperCase() => Assert.That(FileValidator.IsValidFileExtensions("1.JPG", new[] { "JPG" }), Is.True);

    [Test]
    public void FileNameLengthNotAssigned() => Assert.Throws<ArgumentException>(() => FileValidator.IsValidFileNameLength("1.txt", 0));

    [Test]
    public void InvalidFileNameLength() => Assert.That(FileValidator.IsValidFileNameLength("1.txt", 1), Is.False);

    [Test]
    public void ValidFileNameLength() => Assert.That(FileValidator.IsValidFileNameLength("1.jpg", 50), Is.True);

    [Test]
    public void FileSizeNotAssigned() => Assert.Throws<ArgumentException>(() => FileValidator.IsValidFileSize(1, 0));

    [Test]
    public void InvalidFileSize() => Assert.That(FileValidator.IsValidFileSize(1, 1), Is.False);

    [Test]
    public void ValidFileSize() => Assert.That(FileValidator.IsValidFileSize(1, 50), Is.True);

    [Test]
    public void FileNameSpecialSymbolsNotAssigned() => Assert.Throws<ArgumentException>(() => FileValidator.IsValidFileNameSpecialSymbols("1.txt", null));

    [Test]
    public void InvalidFileNameSpecialSymbols() => Assert.That(FileValidator.IsValidFileNameSpecialSymbols("1#.txt", new[] { "#" }), Is.False);

    [Test]
    public void ValidFileNameSpecialSymbols() => Assert.That(FileValidator.IsValidFileNameSpecialSymbols("1.jpg", new[] { "#" }), Is.True);

    [Test]
    public void InvalidFileExtensionWithStream()
    {
        const string fileName = "fake.jpg";
        using var stream = File.OpenRead(Path.Combine(RootPath, fileName));
        Assert.That(FileValidator.IsValidFileExtension(fileName, stream), Is.False);
    }

    [Test]
    public void ValidFileExtensionWithStream()
    {
        const string fileName = "real.jpg";
        using var stream = File.OpenRead(Path.Combine(RootPath, fileName));
        Assert.That(FileValidator.IsValidFileExtension(fileName, stream), Is.True);
    }

    [Test]
    public void InvalidFileExtensionWithBytes()
    {
        const string fileName = "fake.jpg";
        var fileBytes = File.ReadAllBytes(Path.Combine(RootPath, fileName));
        Assert.That(FileValidator.IsValidFileExtension(fileName, fileBytes), Is.False);
    }

    [Test]
    public void ValidFileExtensionWithBytes()
    {
        const string fileName = "real.jpg";
        var fileBytes = File.ReadAllBytes(Path.Combine(RootPath, fileName));
        Assert.That(FileValidator.IsValidFileExtension(fileName, fileBytes), Is.True);
    }

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
