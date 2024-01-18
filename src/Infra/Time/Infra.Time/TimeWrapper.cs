using Infra.Core.Time.Abstractions;

namespace Infra.Time;

public class TimeWrapper : ITimeWrapper
{
    public DateTime Now => TimeProvider.System.GetLocalNow().DateTime;

    public DateTime UtcNow => TimeProvider.System.GetUtcNow().DateTime;
}
