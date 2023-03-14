using System.Globalization;
using Infra.Core.Extensions;

namespace Infra.Core.FileAccess.Validators;

public static class FileValidator
{
    public static bool IsValidFileExtensions(string fileName, string[] extensions)
    {
        if (extensions is null)
            throw new ArgumentException("File extensions not assigned.");

        extensions = extensions.Select(extension =>
        {
            extension = extension.ToLower(CultureInfo.InvariantCulture);
            return extension[..1] is "." ? extension : $".{extension}";
        }).ToArray();

        return extensions.Contains(Path.GetExtension(fileName).ToLower(CultureInfo.InvariantCulture));
    }

    public static bool IsValidFileNameLength(string fileName, int length)
    {
        if (length is 0)
            throw new ArgumentException("File name length not assigned.");

        return length > fileName.Length;
    }

    public static bool IsValidFileSize(long fileLength, long size)
    {
        if (size is 0)
            throw new ArgumentException("File size not assigned.");

        return size > fileLength;
    }

    public static bool IsValidFileNameSpecialSymbols(string fileName, string[] symbols)
    {
        if (symbols is null)
            throw new ArgumentException("File name special symbols not assigned.");

        return !symbols.Any(fileName.Contains);
    }

    public static bool IsValidFileExtension(string fileName, Stream fileStream, byte[] allowedChars = null)
        => IsValidFileExtension(fileName, fileStream.ToBytes(), allowedChars);

    public static bool IsValidFileExtension(string fileName, byte[] fileBytes, byte[] allowedChars = null)
        => FileExtensionValidator.IsValidFileExtension(fileName, fileBytes, allowedChars);
}
