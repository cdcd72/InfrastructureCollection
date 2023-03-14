namespace Infra.Core.Time.Abstractions;

public interface ITimeWrapper
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}
