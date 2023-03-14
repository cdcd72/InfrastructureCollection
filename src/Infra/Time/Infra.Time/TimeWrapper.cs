using Infra.Core.Time;
using Infra.Core.Time.Abstractions;

namespace Infra.Time
{
    public class TimeWrapper : ITimeWrapper
    {
        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
