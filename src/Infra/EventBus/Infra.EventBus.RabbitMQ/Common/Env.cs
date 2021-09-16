using Microsoft.Extensions.Configuration;

namespace Infra.EventBus.RabbitMQ.Common
{
    public class Env
    {
        private const string SectionName = "EventBus";

        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int RetryCount { get; set; }

        public string QueueName { get; set; }

        #region Constructor

        public Env(IConfiguration config) => InitEnv(config);

        #endregion

        #region Private Method

        private void InitEnv(IConfiguration config)
        {
            Host = config.GetValue<string>($"{SectionName}:{nameof(Host)}");
            Port = config.GetValue<int>($"{SectionName}:{nameof(Port)}");
            UserName = config.GetValue<string>($"{SectionName}:{nameof(UserName)}");
            Password = config.GetValue<string>($"{SectionName}:{nameof(Password)}");
            RetryCount = GetRetryCount(config);
            QueueName = config.GetValue<string>($"{SectionName}:{nameof(QueueName)}");
        }

        private int GetRetryCount(IConfiguration config)
        {
            var retryCount = config.GetValue<int>($"{SectionName}:{nameof(RetryCount)}");

            return retryCount > 0 ? retryCount : 5;
        }

        #endregion
    }
}
