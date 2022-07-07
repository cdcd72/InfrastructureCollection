using Infra.Core.Hash.Abstractions;
using Infra.Core.Hash.Enums;
using Infra.Core.Hash.Models;
using NUnit.Framework;

namespace Infra.Hash.IntegrationTest;

public class HashFactoryTests
{
    private readonly IHashFactory _hashFactory;

    public HashFactoryTests()
    {
        var startup = new Startup();

        _hashFactory = startup.GetService<IHashFactory>();
    }

    [Test]
    public void CreateHashAlgorithmFail() =>
        Assert.Throws<MissingMethodException>(() => _hashFactory.Create(new HashOptions
        {
            Type = HashType.UnKnown
        }));

    [Test]
    public void CreateHashAlgorithmSuccess()
    {
        var hasher = _hashFactory.Create(new HashOptions
        {
            Type = HashType.Sha512
        });

        Assert.That(hasher, Is.Not.Null);
    }
}
