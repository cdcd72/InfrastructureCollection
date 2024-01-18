namespace Infra.EventBus.RabbitMQ.Configuration;

public class ConnectionSettings
{
    public const string SectionName = "EventBus:RabbitMQ:Connection";

    /// <summary>
    /// RabbitMQ host name
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// RabbitMQ host port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// RabbitMQ user name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// RabbitMQ user password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// RabbitMQ connection retry count
    /// </summary>
    public int RetryCount { get; set; }
}