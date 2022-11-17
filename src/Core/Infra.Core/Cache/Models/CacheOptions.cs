namespace Infra.Core.Cache.Models;

public class CacheOptions
{
    public TimeSpan? SlidingExpiration { get; set; }

    public DateTimeOffset? AbsoluteExpiration { get; set; }

    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
}
