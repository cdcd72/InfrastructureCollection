namespace Infra.Time.Configuration
{
    public class Settings
    {
        public const string SectionName = "Time";

        /// <summary>
        /// 正則運算逾時時間（單位：毫秒）
        /// </summary>
        public int ExpressionMatchTimeout { get; set; }
    }
}
