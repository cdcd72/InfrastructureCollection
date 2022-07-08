using System.Text;
using Infra.Core.Hash.Abstractions;
using Infra.Core.Hash.Enums;
using Infra.Core.Hash.Models;
using NUnit.Framework;

namespace Infra.Hash.IntegrationTest.HmacAlgorithm;

public class HmacSha512Tests
{
    private readonly IHmacAlgorithm _hasher;

    public HmacSha512Tests()
    {
        var startup = new Startup();

        var hashFactory = startup.GetService<IHmacFactory>();

        _hasher = hashFactory.Create(new HmacOptions
        {
            Type = HmacType.HmacSha512
        });
    }

    [Test]
    public void HashTextSuccess()
    {
        const string text = "test";

        var tuple = _hasher.Hash(text);

        Assert.That(tuple.hashedText, Is.Not.EqualTo(text));
    }

    [Test]
    public void HashTextWithKeySuccess()
    {
        const string text = "test";

        var hashedText = _hasher.Hash(text, Encoding.UTF8.GetBytes("zpw!AVkxEMar@S%nUKrFSG?6p$7S?$%@"));

        Assert.That(hashedText, Is.Not.EqualTo(text));
    }

    [Test]
    public void HashBytesSuccess()
    {
        var bytes = Encoding.UTF8.GetBytes("test");

        var tuple = _hasher.Hash(bytes);

        Assert.That(tuple.hashedBytes, Is.Not.Length.EqualTo(bytes.Length));
    }

    [Test]
    public void HashBytesWithKeySuccess()
    {
        var bytes = Encoding.UTF8.GetBytes("test");

        var hashedBytes = _hasher.Hash(bytes, Encoding.UTF8.GetBytes("zpw!AVkxEMar@S%nUKrFSG?6p$7S?$%@"));

        Assert.That(hashedBytes, Is.Not.Length.EqualTo(bytes.Length));
    }

    [Test]
    public void VerifyHashedTextSuccess()
    {
        const string text = "test";

        var tuple = _hasher.Hash(text);

        Assert.That(_hasher.Verify(text, tuple.hashedText, tuple.key), Is.True);
    }

    [Test]
    public void VerifyHashedTextWithKeySuccess()
    {
        const string text = "test";

        var key = Encoding.UTF8.GetBytes("zpw!AVkxEMar@S%nUKrFSG?6p$7S?$%@");

        var hashedText = _hasher.Hash(text, key);

        Assert.That(_hasher.Verify(text, hashedText, key), Is.True);
    }

    [Test]
    public void VerifyHashedBytesSuccess()
    {
        var bytes = Encoding.UTF8.GetBytes("test");

        var tuple = _hasher.Hash(bytes);

        Assert.That(_hasher.Verify(bytes, tuple.hashedBytes, tuple.key), Is.True);
    }

    [Test]
    public void VerifyHashedBytesWithKeySuccess()
    {
        var bytes = Encoding.UTF8.GetBytes("test");

        var key = Encoding.UTF8.GetBytes("zpw!AVkxEMar@S%nUKrFSG?6p$7S?$%@");

        var hashedBytes = _hasher.Hash(bytes, key);

        Assert.That(_hasher.Verify(bytes, hashedBytes, key), Is.True);
    }
}
