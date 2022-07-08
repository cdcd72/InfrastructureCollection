namespace Infra.Core.Hash.Abstractions;

public interface IHashAlgorithm
{
    string Hash(string text);

    byte[] Hash(byte[] bytes);

    bool Verify(string text, string hashedText);

    bool Verify(byte[] bytes, byte[] hashedBytes);
}
