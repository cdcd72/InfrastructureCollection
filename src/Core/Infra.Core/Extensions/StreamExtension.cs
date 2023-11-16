namespace Infra.Core.Extensions;

public static class StreamExtension
{
    public static byte[] ToBytes(this Stream stream)
    {
        var buffer = new byte[8 * 1024];

        using var ms = new MemoryStream();

        int read;

        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, read);
        }

        return ms.ToArray();
    }

    public static async Task<byte[]> ToBytesAsync(this Stream stream)
    {
        if (stream is MemoryStream ms)
            return ms.ToArray();

        using var outputStream = new MemoryStream();

        await stream.CopyToAsync(outputStream);

        return outputStream.ToArray();
    }
}
