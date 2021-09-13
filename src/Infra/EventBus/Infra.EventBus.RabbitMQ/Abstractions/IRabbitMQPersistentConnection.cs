using System;
using RabbitMQ.Client;

namespace Infra.EventBus.RabbitMQ.Abstractions
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        IModel CreateModel();

        bool TryConnect();
    }
}
