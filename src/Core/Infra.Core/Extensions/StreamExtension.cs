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
}
