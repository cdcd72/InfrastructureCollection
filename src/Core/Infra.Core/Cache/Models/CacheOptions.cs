namespace Infra.Core.Cache.Models;

public class CacheOptions
{
    public TimeSpan? SlidingExpiration { get; init; }

    public DateTimeOffset? AbsoluteExpiration { get; init; }

    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }
}
