namespace Infra.Core.Crypto.Abstractions;

public interface ICryptoAlgorithm
{
    string Encrypt(string text);

    byte[] Encrypt(byte[] bytes);

    string Decrypt(string encryptedText);

    byte[] Decrypt(byte[] encryptedBytes);
}
