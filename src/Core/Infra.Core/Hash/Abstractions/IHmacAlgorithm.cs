namespace Infra.Core.Hash.Abstractions;

public interface IHmacAlgorithm
{
    (string hashedText, byte[] key) Hash(string text);

    string Hash(string text, byte[] key);

    (byte[] hashedBytes, byte[] key) Hash(byte[] bytes);

    byte[] Hash(byte[] bytes, byte[] key);

    bool Verify(string text, string hashedText, byte[] key);

    bool Verify(byte[] bytes, byte[] hashedBytes, byte[] key);
}
