using Infra.Core.Crypto.Abstractions;
using Infra.Core.Crypto.Enums;
using Infra.Core.Crypto.Models;
using NUnit.Framework;

namespace Infra.Crypto.IntegrationTest;

public class CryptoFactoryTests
{
    private readonly ICryptoFactory cryptoFactory;

    public CryptoFactoryTests()
    {
        var startup = new Startup();

        cryptoFactory = startup.GetService<ICryptoFactory>();
    }

    [Test]
    public void CreateCryptoAlgorithmFail()
    {
        const string key = "wgaPBHA8ab7Jb6viQ34XZjXEKO2Rn7USYcTE6BnYv+Y=";
        const string iv = "Y0xnm45ygKFcSU62fmLBww==";

        Assert.Throws<MissingMethodException>(() => cryptoFactory.Create(new CryptoOptions
        {
            Type = CryptoType.UnKnown,
            KeyPair = CryptoKeyPair.Parse(key, iv)
        }));
    }

    [Test]
    public void CreateCryptoAlgorithmSuccess()
    {
        const string key = "wgaPBHA8ab7Jb6viQ34XZjXEKO2Rn7USYcTE6BnYv+Y=";
        const string iv = "Y0xnm45ygKFcSU62fmLBww==";

        var crypto = cryptoFactory.Create(new CryptoOptions
        {
            Type = CryptoType.Aes,
            KeyPair = CryptoKeyPair.Parse(key, iv)
        });

        Assert.That(crypto, Is.Not.Null);
    }
}
