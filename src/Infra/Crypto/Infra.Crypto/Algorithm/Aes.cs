using System.Text;
using Infra.Core.Crypto.Abstractions;

namespace Infra.Crypto.Algorithm;

public class Aes : CipherBase, ICryptoAlgorithm
{
    private readonly byte[] key;
    private readonly byte[] iv;

    public Aes(byte[] key, byte[] iv)
    {
        this.key = key;
        this.iv = iv;
    }

    public string Encrypt(string text)
    {
        using var aes = System.Security.Cryptography.Aes.Create();

        SetCryptoTransform(aes.CreateEncryptor(key, iv));

        return Convert.ToBase64String(Cipher(Encoding.UTF8.GetBytes(text)));
    }

    public byte[] Encrypt(byte[] bytes)
    {
        using var aes = System.Security.Cryptography.Aes.Create();

        SetCryptoTransform(aes.CreateEncryptor(key, iv));

        return Cipher(bytes);
    }

    public string Decrypt(string encryptedText)
    {
        using var aes = System.Security.Cryptography.Aes.Create();

        SetCryptoTransform(aes.CreateDecryptor(key, iv));

        return Encoding.UTF8.GetString(Cipher(Convert.FromBase64String(encryptedText)));
    }

    public byte[] Decrypt(byte[] encryptedBytes)
    {
        using var aes = System.Security.Cryptography.Aes.Create();

        SetCryptoTransform(aes.CreateDecryptor(key, iv));

        return Cipher(encryptedBytes);
    }
}
