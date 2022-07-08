using Infra.Core.Hash.Abstractions;
using Infra.Core.Hash.Enums;
using Infra.Core.Hash.Models;
using NUnit.Framework;

namespace Infra.Hash.IntegrationTest;

public class HmacFactoryTests
{
    private readonly IHmacFactory _hashFactory;

    public HmacFactoryTests()
    {
        var startup = new Startup();

        _hashFactory = startup.GetService<IHmacFactory>();
    }

    [Test]
    public void CreateHmacAlgorithmFail() =>
        Assert.Throws<MissingMethodException>(() => _hashFactory.Create(new HmacOptions
        {
            Type = HmacType.UnKnown
        }));

    [Test]
    public void CreateHmacAlgorithmSuccess()
    {
        var hasher = _hashFactory.Create(new HmacOptions
        {
            Type = HmacType.HmacSha512
        });

        Assert.That(hasher, Is.Not.Null);
    }
}
