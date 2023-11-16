using Infra.Core.Time.Abstractions;
using NUnit.Framework;

namespace Infra.Time.IntegrationTest;

public class TimeWrapperTests
{
    private readonly ITimeWrapper wrapper;

    public TimeWrapperTests()
    {
        var startup = new Startup();

        wrapper = startup.GetService<ITimeWrapper>();
    }

    [Test]
    public void GetNow() => Assert.That($"{wrapper.Now:yyyy/mm/dd hh:mm:ss}", Is.EqualTo($"{DateTime.Now:yyyy/mm/dd hh:mm:ss}"));

    [Test]
    public void GetUtcNow() => Assert.That($"{wrapper.UtcNow:yyyy/mm/dd hh:mm:ss}", Is.EqualTo($"{DateTime.UtcNow:yyyy/mm/dd hh:mm:ss}"));
}
