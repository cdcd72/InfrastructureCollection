using Infra.Core.Hash.Abstractions;
using Infra.Core.Hash.Enums;
using Infra.Core.Hash.Models;
using Infra.Hash.HmacAlgorithm;

namespace Infra.Hash;

public class HmacFactory : IHmacFactory
{
    public IHmacAlgorithm Create(HmacOptions hmacOptions) =>
        hmacOptions.Type switch
        {
            HmacType.HmacSha256 => new HmacSha256(),
            HmacType.HmacSha512 => new HmacSha512(),
            _ => throw new MissingMethodException()
        };
}
