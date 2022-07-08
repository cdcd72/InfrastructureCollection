using System.Security.Cryptography;
using System.Text;
using Infra.Core.Hash.Abstractions;

namespace Infra.Hash.Algorithm;

public class Sha512 : IHashAlgorithm
{
    public string Hash(string text) => HashValue(text);

    public byte[] Hash(byte[] bytes) => HashBytes(bytes);

    public bool Verify(string text, string hashedText) => HashValue(text) == hashedText;

    public bool Verify(byte[] bytes, byte[] hashedBytes) => !HashBytes(bytes).Where((b, i) => b != hashedBytes[i]).Any();

    #region Private Method

    private static string HashValue(string text) => Convert.ToBase64String(HashBytes(Encoding.UTF8.GetBytes(text)));

    private static byte[] HashBytes(byte[] bytes)
    {
        using var sha512 = SHA512.Create();

        sha512.Initialize();

        return sha512.ComputeHash(bytes);
    }

    #endregion
}
