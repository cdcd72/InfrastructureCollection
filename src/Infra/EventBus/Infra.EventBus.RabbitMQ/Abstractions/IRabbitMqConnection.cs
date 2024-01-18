using RabbitMQ.Client;

namespace Infra.EventBus.RabbitMQ.Abstractions;

public interface IRabbitMqConnection : IDisposable
{
    bool IsConnected { get; }

    IModel CreateChannel();

    bool TryConnect();
}
