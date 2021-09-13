using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac;
using Infra.Core.EventBus;
using Infra.Core.EventBus.Abstractions;
using Infra.Core.EventBus.Events;
using Infra.Core.EventBus.Extensions;
using Infra.EventBus.RabbitMQ.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infra.EventBus.RabbitMQ
{
    public class RabbitMQBus : IEventBus, IDisposable
    {
        const string BROKER_NAME = "event_bus";
        const string AUTOFAC_SCOPE_NAME = "event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMQBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;
        private bool _disposed;

        #region Constructor

        public RabbitMQBus(
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQBus> logger,
            IEventBusSubscriptionsManager subsManager,
            ILifetimeScope autofac,
            string queueName = null,
            int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _autofac = autofac;
            _queueName = queueName;
            _retryCount = retryCount;
            _consumerChannel = CreateConsumerChannel();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        #endregion

        public void Publish(IntegrationEvent integrationEvent) => throw new NotImplementedException();

        public void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = _subsManager.GetEventName<TIntegrationEvent>();

            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}.", eventName, typeof(TIntegrationEventHandler).GetGenericTypeName());

            _subsManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();

            StartBasicConsume();
        }

        public void SubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler => throw new NotImplementedException();

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = _subsManager.GetEventName<TIntegrationEvent>();

            _logger.LogInformation("Unsubscribing from event {EventName}.", eventName);

            _subsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void UnsubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler => throw new NotImplementedException();

        #region Private Method

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            using var channel = _persistentConnection.CreateModel();

            channel.QueueUnbind(queue: _queueName,
                exchange: BROKER_NAME,
                routingKey: eventName);

            if (_subsManager.IsEmpty)
            {
                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }

        private void DoInternalSubscription(string eventName)
        {
            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!_persistentConnection.IsConnected)
                    _persistentConnection.TryConnect();

                _consumerChannel.QueueBind(
                    queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            _logger.LogTrace("Creating RabbitMQ consumer channel.");

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(
                exchange: BROKER_NAME,
                type: "direct");

            channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel.");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume.");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null .");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

                var subscriptions = _subsManager.GetHandlersForEvent(eventName);

                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        if (scope.ResolveOptional(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler)
                            continue;

                        using dynamic eventData = JsonDocument.Parse(message);

                        await Task.Yield();

                        await handler.HandleAsync(eventData);
                    }
                    else
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);

                        if (handler is null)
                            continue;

                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }

        #endregion

        #region Dispose

        ~RabbitMQBus() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // TODO: managed dispose
            }

            _disposed = true;
        }

        #endregion
    }
}
