using System;
using RabbitMQ.Client;

namespace Infra.EventBus.RabbitMQ.Abstractions
{
    public interface IRabbitMQConnection : IDisposable
    {
        bool IsConnected { get; }

        IModel CreateChannel();

        bool TryConnect();
    }
}
