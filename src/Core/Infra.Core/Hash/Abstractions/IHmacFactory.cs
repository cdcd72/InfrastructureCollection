using Infra.Core.Hash.Models;

namespace Infra.Core.Hash.Abstractions;

public interface IHmacFactory
{
    IHmacAlgorithm Create(HmacOptions hmacOptions);
}
