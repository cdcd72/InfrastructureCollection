using System.Security.Cryptography;

namespace Infra.Crypto.Algorithm;

public class CipherBase
{
    private ICryptoTransform cryptoTransform;

    internal void SetCryptoTransform(ICryptoTransform transform) => cryptoTransform = transform;

    internal byte[] Cipher(byte[] bytes)
    {
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }
}
