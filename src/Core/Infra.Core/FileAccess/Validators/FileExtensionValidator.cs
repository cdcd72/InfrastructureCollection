namespace Infra.Core.FileAccess.Validators;

/// <summary>
/// FileExtensionValidator from https://github.com/rainmakerho/FileExtensionValidation
/// </summary>
public static class FileExtensionValidator
{
    // https://en.wikipedia.org/wiki/List_of_file_signatures
    // https://www.filesignatures.net/index.php
    // https://www.garykessler.net/library/file_sigs.html
    // https://asecuritysite.com/forensics/magic
    private static readonly Dictionary<string, List<(byte[] Signature, int Offset, int Skip, byte[] SecondSignature)>> FileSignature
        = new()
        {
            {   ".DOC",
                new List<(byte[], int, int, byte[])>
                {
                    ([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".DOCX",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {   ".PDF",
                new List<(byte[], int, int, byte[])>
                {
                    ([0x25, 0x50, 0x44, 0x46], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".ZIP",
                new List<(byte[], int, int, byte[])>
                {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x4C, 0x49, 0x54, 0x55], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x53, 0x70, 0x58], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>()),
                    ([0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".PNG",
                new List<(byte[], int, int, byte[])> {
                    ([0x89, 0x50, 0x4E, 0x47], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE0], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE1], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE8], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".JPG",
                new List<(byte[], int, int, byte[])>
                {
                    ([0xFF, 0xD8, 0xFF, 0xE0], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE1], 0, 6, [0x45, 0x78, 0x69, 0x66, 0x00, 0x00]),
                    ([0xFF, 0xD8, 0xFF, 0xE8], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE2], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE3], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xDB], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xEE], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".JPEG",
                new List<(byte[], int, int, byte[])>
                {
                    ([0xFF, 0xD8, 0xFF, 0xE0], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE1], 0, 6, [0x45, 0x78, 0x69, 0x66, 0x00, 0x00]),
                    ([0xFF, 0xD8, 0xFF, 0xE8], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE2], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xE3], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xDB], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xD8, 0xFF, 0xEE], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".XLS",
                new List<(byte[], int, int, byte[])>
                {
                    ([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1], 0, 0, Array.Empty<byte>()),
                    ([0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00], 0, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x10], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x1F], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x22], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x23], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x28], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x29], 512, 0, Array.Empty<byte>())
                }
            },
            {
                ".XLSX",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {   ".GIF",
                new List<(byte[], int, int, byte[])>
                {
                    ([0x47, 0x49, 0x46, 0x38], 0, 0, Array.Empty<byte>())
                }
            },
            {   ".7Z",
                new List<(byte[], int, int, byte[])>
                {
                    ([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C], 0, 0, Array.Empty<byte>())
                }
            },
            {   ".BMP",
                new List<(byte[], int, int, byte[])>
                {
                    ([0x42, 0x4D], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".TIF",
                new List<(byte[], int, int, byte[])> {
                    ([0x49, 0x20, 0x49], 0, 0, Array.Empty<byte>()),
                    ([0x49, 0x49, 0x2A, 0x00], 0, 0, Array.Empty<byte>()),
                    ([0x4D, 0x4D, 0x00, 0x2A], 0, 0, Array.Empty<byte>()),
                    ([0x4D, 0x4D, 0x00, 0x2B], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".TIFF",
                new List<(byte[], int, int, byte[])> {
                    ([0x49, 0x20, 0x49], 0, 0, Array.Empty<byte>()),
                    ([0x49, 0x49, 0x2A, 0x00], 0, 0, Array.Empty<byte>()),
                    ([0x4D, 0x4D, 0x00, 0x2A], 0, 0, Array.Empty<byte>()),
                    ([0x4D, 0x4D, 0x00, 0x2B], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".RTF",
                new List<(byte[], int, int, byte[])> {
                    ([0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31],0, 0, Array.Empty<byte>())
                }
            },
            {
                ".PPT",
                new List<(byte[], int, int, byte[])> {
                    ([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1], 0, 0, Array.Empty<byte>()),
                    ([0x00, 0x6E, 0x1E, 0xF0], 512, 0, Array.Empty<byte>()),
                    ([0x0F, 0x00, 0xE8, 0x03], 512, 0, Array.Empty<byte>()),
                    ([0xA0, 0x46, 0x1D, 0xF0], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x0E, 0x00, 0x00, 0x00], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00], 512, 0, Array.Empty<byte>()),
                    ([0xFD, 0xFF, 0xFF, 0xFF, 0x43, 0x00, 0x00, 0x00], 512, 0, Array.Empty<byte>())
                }
            },
            {
                ".PPTX",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".ODF",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".ODG",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".ODP",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".ODS",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".ODT",
                new List<(byte[], int, int, byte[])> {
                    ([0x50, 0x4B, 0x03, 0x04], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x05, 0x06], 0, 0, Array.Empty<byte>()),
                    ([0x50, 0x4B, 0x07, 0x08], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".MPG",
                new List<(byte[], int, int, byte[])> {
                    ([0x00, 0x00, 0x01, 0xBA], 0, 0, Array.Empty<byte>()),
                    ([0x00, 0x00, 0x01, 0xB3], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".MPEG",
                new List<(byte[], int, int, byte[])> {
                    ([0x00, 0x00, 0x01, 0xBA], 0, 0, Array.Empty<byte>()),
                    ([0x00, 0x00, 0x01, 0xB3], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".AVI",
                new List<(byte[], int, int, byte[])> {
                    ([0x52, 0x49, 0x46, 0x46], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".WMV",
                new List<(byte[], int, int, byte[])> {
                    ([0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".RM",
                new List<(byte[], int, int, byte[])> {
                    ([0x2E, 0x52, 0x4D, 0x46], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".MOV",
                new List<(byte[], int, int, byte[])> {
                    ([0x66, 0x74, 0x79, 0x70, 0x71, 0x74, 0x20, 0x20], 4, 0, Array.Empty<byte>()),
                    ([0x6D, 0x6F, 0x6F, 0x76], 4, 0, Array.Empty<byte>()),
                    ([0x66, 0x72, 0x65, 0x65], 4, 0, Array.Empty<byte>()),
                    ([0x6D, 0x64, 0x61, 0x74], 4, 0, Array.Empty<byte>()),
                    ([0x77, 0x69, 0x64, 0x65], 4, 0, Array.Empty<byte>()),
                    ([0x70, 0x6E, 0x6F, 0x74], 4, 0, Array.Empty<byte>()),
                    ([0x73, 0x6B, 0x69, 0x70], 4, 0, Array.Empty<byte>())
                }
            },
            {
                ".MKV",
                new List<(byte[], int, int, byte[])> {
                    ([0x1A, 0x45, 0xDF, 0xA3], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".RAR",
                new List<(byte[], int, int, byte[])>{
                    ([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".3GP",
                new List<(byte[], int, int, byte[])>{
                    ([0x00, 0x00, 0x00, 0x14, 0x66, 0x74, 0x79, 0x70], 0, 0, Array.Empty<byte>()),
                    ([0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70], 0, 0, Array.Empty<byte>()),
                    ([0x66, 0x74, 0x79, 0x70, 0x33, 0x67, 0x70], 4, 0, Array.Empty<byte>())
                }
            },
            {
                ".MP3",
                new List<(byte[], int, int, byte[])>{
                    ([0x49, 0x44, 0x33], 0, 0, Array.Empty<byte>()),
                    ([0xFF, 0xFB], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".WAV",
                new List<(byte[], int, int, byte[])>{
                    ([0x52, 0x49, 0x46, 0x46], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".MP4",
                new List<(byte[], int, int, byte[])>{
                    ([0x66, 0x74, 0x79, 0x70], 4, 0, Array.Empty<byte>())
                }

            },
            {
                ".M4V",
                new List<(byte[], int, int, byte[])>{
                    ([0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32], 4, 0, Array.Empty<byte>()),
                    ([0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x56, 0x20], 4, 0, Array.Empty<byte>())
                }
            },
            {
                ".MSI",
                new List<(byte[], int, int, byte[])>{
                    ([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1], 0, 0, Array.Empty<byte>())
                }
            },
            {
                ".HTML",
                new List<(byte[], int, int, byte[])>{
                    ([0x3C, 0x21, 0x44, 0x4F, 0x43, 0x54, 0x59, 0x50, 0x45, 0x20, 0x68, 0x74, 0x6D, 0x6C, 0x3E], 0, 0, Array.Empty<byte>())
                }
            }
        };

    public static bool IsValidFileExtension(string fileName, byte[] fileData, byte[] allowedChars)
    {
        if (string.IsNullOrEmpty(fileName) || fileData == null || fileData.Length == 0)
            return false;

        var flag = false;
        var ext = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(ext))
            return false;

        ext = ext.ToUpperInvariant();

        if (ext.Equals(".TXT", StringComparison.Ordinal) || ext.Equals(".CSV", StringComparison.Ordinal) || ext.Equals(".PRN", StringComparison.Ordinal))
        {
            foreach (var b in fileData)
            {
                if (b > 0xFF)
                {
                    if (allowedChars != null)
                    {
                        if (!allowedChars.Contains(b))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        if (!FileSignature.TryGetValue(ext, out var sigs))
            return false;

        foreach (var (Signature, Offset, Skip, SecondSignature) in sigs)
        {
            var curFileSig = new byte[Signature.Length];
            Array.Copy(fileData.Skip(Offset).ToArray(), curFileSig, Signature.Length);
            flag = curFileSig.SequenceEqual(Signature);
            if (Skip > 0 && flag)
            {
                var secondFileSig = new byte[SecondSignature.Length];
                Array.Copy(fileData.Skip(Skip).ToArray(), secondFileSig, SecondSignature.Length);
                flag = secondFileSig.SequenceEqual(SecondSignature);
            }
            if (flag)
                break;
        }

        return flag;
    }

    public static bool IsValidFileExtension(string fileName, FileStream fs, byte[] allowedChars)
    {
        if (string.IsNullOrEmpty(fileName) || fs == null || fs.Length == 0)
            return false;

        var flag = false;
        var ext = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(ext))
            return false;

        ext = ext.ToUpperInvariant();

        if (ext.Equals(".TXT", StringComparison.Ordinal) || ext.Equals(".CSV", StringComparison.Ordinal) || ext.Equals(".PRN", StringComparison.Ordinal))
        {
            var fileData = new byte[fs.Length];
            _ = fs.Read(fileData, 0, Convert.ToInt32(fs.Length));

            foreach (var b in fileData)
            {
                if (b > 0x7F)
                {
                    if (allowedChars != null)
                    {
                        if (!allowedChars.Contains(b))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        if (!FileSignature.TryGetValue(ext, out var sigs))
            return false;

        foreach (var (Signature, Offset, Skip, SecondSignature) in sigs)
        {
            var curFileSig = new byte[Signature.Length];
            fs.Position = Offset;
            _ = fs.Read(curFileSig, 0, Signature.Length);
            flag = curFileSig.SequenceEqual(Signature);
            if (Skip > 0 && flag)
            {
                var secondFileSig = new byte[SecondSignature.Length];
                fs.Position = Skip;
                _ = fs.Read(secondFileSig, 0, SecondSignature.Length);
                flag = secondFileSig.SequenceEqual(SecondSignature);
            }
            if (flag)
                break;
        }

        return flag;
    }
}
