namespace Infra.Core.Time.Abstractions;

public interface ITimeSpanHelper
{
    /// <summary>
    /// 取得正則運算逾時時間
    /// </summary>
    /// <param name="timeout">預計逾時時間</param>
    /// <returns></returns>
    public TimeSpan GetExpressionMatchTimeout(TimeSpan? timeout = null);
}
