using Infra.Core.Time.Abstractions;
using NUnit.Framework;

namespace Infra.Time.IntegrationTest;

public class TimeSpanHelperTests
{
    private readonly ITimeSpanHelper helper;

    public TimeSpanHelperTests()
    {
        var startup = new Startup();

        helper = startup.GetService<ITimeSpanHelper>();
    }

    #region GetExpressionMatchTimeout

    [Test]
    public void GetExpressionMatchTimeoutFromConfigSuccess()
    {
        var timeout = helper.GetExpressionMatchTimeout();

        Assert.That(timeout.TotalMilliseconds, Is.EqualTo(3000));
    }

    [Test]
    public void GetExpressionMatchTimeoutFromAssignedValueSuccess()
    {
        var timeout = helper.GetExpressionMatchTimeout(TimeSpan.FromSeconds(5));

        Assert.That(timeout.TotalMilliseconds, Is.EqualTo(5000));
    }

    #endregion
}
