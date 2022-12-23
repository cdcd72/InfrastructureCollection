namespace Infra.Core.Time;

public interface ITimeWrapper
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}