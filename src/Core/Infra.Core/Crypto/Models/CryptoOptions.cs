using Infra.Core.Crypto.Enums;

namespace Infra.Core.Crypto.Models;

public class CryptoOptions
{
    public CryptoType Type { get; set; }

    public CryptoKeyPair KeyPair { get; set; }
}
