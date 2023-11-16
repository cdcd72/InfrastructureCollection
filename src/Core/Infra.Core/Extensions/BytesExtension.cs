namespace Infra.Core.Extensions;

public static class BytesExtension
{
    public static Stream ToStream(this byte[] bytes) => new MemoryStream(bytes);
}
