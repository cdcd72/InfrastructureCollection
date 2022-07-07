using Infra.Core.Hash.Models;

namespace Infra.Core.Hash.Abstractions;

public interface IHashFactory
{
    IHashAlgorithm Create(HashOptions hashOptions);
}
