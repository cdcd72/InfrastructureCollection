using Infra.Core.Generators;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.Generators;

public class RandomNumberGeneratorTests
{
    [Test]
    public void RandomSuccess()
    {
        const int max = 10;
        const int count = 10000;

        var index = 0;
        var pass = true;

        while (index < count)
        {
            pass = RandomNumberGenerator.Next(max) <= max;
            index++;
        }

        Assert.IsTrue(pass);
    }


    [Test]
    public void RandomRangeSuccess()
    {
        const int max = 10;
        const int min = 0;
        const int count = 10000;

        var index = 0;
        var pass = true;

        while (index < count)
        {
            var randomNumber = RandomNumberGenerator.Next(min, max);
            pass = randomNumber is <= max and >= min;
            index++;
        }
        Assert.IsTrue(pass);
    }
}
