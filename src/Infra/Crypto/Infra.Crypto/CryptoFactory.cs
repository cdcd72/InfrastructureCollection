using Infra.Core.Crypto.Abstractions;
using Infra.Core.Crypto.Enums;
using Infra.Core.Crypto.Models;
using Infra.Crypto.Algorithm;

namespace Infra.Crypto;

public class CryptoFactory : ICryptoFactory
{
    public ICryptoAlgorithm Create(CryptoOptions cryptoOptions) =>
        cryptoOptions.Type switch
        {
            CryptoType.Aes => new Aes(cryptoOptions.KeyPair.Key, cryptoOptions.KeyPair.Iv),
            _ => throw new MissingMethodException()
        };
}
