using System.Security.Cryptography;
using System.Text;
using Infra.Core.Crypto.Abstractions;
using Infra.Core.Crypto.Enums;
using Infra.Core.Crypto.Models;
using NUnit.Framework;

namespace Infra.Crypto.IntegrationTest;

public class AesTests
{
    private readonly ICryptoAlgorithm _crypto;

    public AesTests()
    {
        var startup = new Startup();

        var cryptoFactory = startup.GetService<ICryptoFactory>();
        const string key = "wgaPBHA8ab7Jb6viQ34XZjXEKO2Rn7USYcTE6BnYv+Y=";
        const string iv = "Y0xnm45ygKFcSU62fmLBww==";

        _crypto = cryptoFactory.Create(new CryptoOptions
        {
            Type = CryptoType.Aes,
            KeyPair = CryptoKeyPair.Parse(key, iv)
        });
    }

    #region Encrypt

    [Test]
    public void EncryptPlainTextSuccess()
    {
        const string plainText = "test";

        var encryptedText = _crypto.Encrypt(plainText);

        Assert.That(encryptedText, Is.Not.EqualTo(plainText));
    }

    [Test]
    public void EncryptByteArraySuccess()
    {
        const string plainText = "test";

        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        var encryptedBytes = _crypto.Encrypt(plainBytes);

        Assert.That(encryptedBytes, Has.Length.Not.EqualTo(plainBytes.Length));
    }

    #endregion

    #region Decrypt

    [Test]
    public void DecryptEncryptedTextSuccess()
    {
        const string plainText = "test";

        var encryptedText = _crypto.Encrypt(plainText);

        Assert.That(_crypto.Decrypt(encryptedText), Is.EqualTo(plainText));
    }

    [Test]
    public void DecryptEncryptedByteArraySuccess()
    {
        const string plainText = "test";

        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        var encryptedBytes = _crypto.Encrypt(plainBytes);

        var decryptedBytes = _crypto.Decrypt(encryptedBytes);

        Assert.That(decryptedBytes, Has.Length.EqualTo(plainBytes.Length));
    }

    #endregion

    [Test]
    public void CreateAesKeyPair()
    {
        using var aes = Aes.Create();

        aes.GenerateKey();
        aes.GenerateIV();

        var key = Convert.ToBase64String(aes.Key);
        var iv = Convert.ToBase64String(aes.IV);

        Assert.Multiple(() =>
        {
            Assert.That(key, Is.Not.Null);
            Assert.That(iv, Is.Not.Null);
        });
    }
}
