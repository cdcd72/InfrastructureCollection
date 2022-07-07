namespace Infra.Core.Crypto.Abstractions;

public interface ICryptoAlgorithm
{
    string Encrypt(string plainText);

    byte[] Encrypt(byte[] plainBytes);

    string Decrypt(string encryptedText);

    byte[] Decrypt(byte[] encryptedBytes);
}
