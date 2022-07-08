using System.Security.Cryptography;
using System.Text;
using Infra.Core.Hash.Abstractions;

namespace Infra.Hash.HmacAlgorithm;

public class HmacSha512 : IHmacAlgorithm
{
    public (string hashedText, byte[] key) Hash(string text) => HashValue(text);

    public string Hash(string text, byte[] key) => HashValue(text, key).hashedText;

    public (byte[] hashedBytes, byte[] key) Hash(byte[] bytes) => HashBytes(bytes);

    public byte[] Hash(byte[] bytes, byte[] key) => HashBytes(bytes, key).hashedBytes;

    public bool Verify(string text, string hashedText, byte[] key) => HashValue(text, key).hashedText == hashedText;

    public bool Verify(byte[] bytes, byte[] hashedBytes, byte[] key) => !HashBytes(bytes, key).hashedBytes.Where((b, i) => b != hashedBytes[i]).Any();

    #region Private Method

    private static (string hashedText, byte[] key) HashValue(string text, byte[]? key = null)
    {
        var tuple = HashBytes(Encoding.UTF8.GetBytes(text), key);

        return (Convert.ToBase64String(tuple.hashedBytes), tuple.key);
    }

    private static (byte[] hashedBytes, byte[] key) HashBytes(byte[] bytes, byte[]? key = null)
    {
        using var hmacSha512 = key is not null ? new HMACSHA512(key) : new HMACSHA512();

        hmacSha512.Initialize();

        return (hmacSha512.ComputeHash(bytes), hmacSha512.Key);
    }

    #endregion
}
