namespace Infra.EventBus.RabbitMQ.Configuration
{
    public class Settings
    {
        public const string SectionName = "EventBus:RabbitMQ";

        /// <summary>
        /// RabbitMQ queue name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// RabbitMQ event publish retry count
        /// </summary>
        public int RetryCount { get; set; }
    }
}
