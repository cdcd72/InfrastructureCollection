using Infra.Core.Hash.Abstractions;
using Infra.Core.Hash.Enums;
using Infra.Core.Hash.Models;
using Infra.Hash.Algorithm;

namespace Infra.Hash;

public class HashFactory : IHashFactory
{
    public IHashAlgorithm Create(HashOptions hashOptions) =>
        hashOptions.Type switch
        {
            HashType.Sha384 => new Sha384(),
            HashType.Sha512 => new Sha512(),
            _ => throw new MissingMethodException()
        };
}
