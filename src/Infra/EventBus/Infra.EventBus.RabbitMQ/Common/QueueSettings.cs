namespace Infra.EventBus.RabbitMQ.Common
{
    public class QueueSettings
    {
        public string Type { get; set; }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public string Name { get; set; }
    }
}
