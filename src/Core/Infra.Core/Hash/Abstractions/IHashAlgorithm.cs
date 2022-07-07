namespace Infra.Core.Hash.Abstractions;

public interface IHashAlgorithm
{
    string Hash(string text);

    byte[] Hash(byte[] bytes);

    bool Verify(string hashedText, string text);

    bool Verify(byte[] hashedBytes, byte[] bytes);
}
