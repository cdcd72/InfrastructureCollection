using System.IO.Compression;
using Infra.Core.FileAccess.Enums;

namespace Infra.Core.Extensions;

public static class IntExtension
{
    public static CompressionLevel ToCompressionLevel(this int level)
    {
        var fileCompressionLevel = Enum.Parse<FileCompressionLevel>($"{level}");

        var compressionLevel = fileCompressionLevel switch
        {
            FileCompressionLevel.Level0 => CompressionLevel.NoCompression,
            FileCompressionLevel.Level1 => CompressionLevel.Fastest,
            FileCompressionLevel.Level2 => CompressionLevel.Optimal,
            FileCompressionLevel.Level3 => CompressionLevel.Optimal,
            FileCompressionLevel.Level4 => CompressionLevel.Optimal,
            FileCompressionLevel.Level5 => CompressionLevel.Optimal,
            FileCompressionLevel.Level6 => CompressionLevel.Optimal,
            FileCompressionLevel.Level7 => CompressionLevel.Optimal,
            FileCompressionLevel.Level8 => CompressionLevel.Optimal,
            FileCompressionLevel.Level9 => CompressionLevel.SmallestSize,
            _ => CompressionLevel.SmallestSize
        };

        return compressionLevel;
    }
}
