using Infra.Core.Hash.Abstractions;
using Infra.Core.Hash.Enums;
using Infra.Core.Hash.Models;
using NUnit.Framework;

namespace Infra.Hash.IntegrationTest;

public class HmacFactoryTests
{
    private readonly IHmacFactory hashFactory;

    public HmacFactoryTests()
    {
        var startup = new Startup();

        hashFactory = startup.GetService<IHmacFactory>();
    }

    [Test]
    public void CreateHmacAlgorithmFail() =>
        Assert.Throws<MissingMethodException>(() => hashFactory.Create(new HmacOptions
        {
            Type = HmacType.UnKnown
        }));

    [Test]
    public void CreateHmacAlgorithmSuccess()
    {
        var hasher = hashFactory.Create(new HmacOptions
        {
            Type = HmacType.HmacSha512
        });

        Assert.That(hasher, Is.Not.Null);
    }
}
