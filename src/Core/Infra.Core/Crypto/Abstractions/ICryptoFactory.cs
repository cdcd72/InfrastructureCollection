using Infra.Core.Crypto.Models;

namespace Infra.Core.Crypto.Abstractions;

public interface ICryptoFactory
{
    ICryptoAlgorithm Create(CryptoOptions cryptoOptions);
}
