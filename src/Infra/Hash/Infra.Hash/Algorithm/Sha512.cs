using System.Security.Cryptography;
using System.Text;
using Infra.Core.Hash.Abstractions;

namespace Infra.Hash.Algorithm;

public class Sha512 : IHashAlgorithm
{
    public string Hash(string text) => HashValue(text);

    public byte[] Hash(byte[] bytes) => HashBytes(bytes);

    public bool Verify(string hashedText, string text) => hashedText == HashValue(text);

    public bool Verify(byte[] hashedBytes, byte[] bytes) => hashedBytes.Length == HashBytes(bytes).Length;

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
