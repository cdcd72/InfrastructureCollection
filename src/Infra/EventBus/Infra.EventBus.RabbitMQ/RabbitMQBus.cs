using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac;
using Infra.Core.EventBus;
using Infra.Core.EventBus.Abstractions;
using Infra.Core.EventBus.Events;
using Infra.Core.EventBus.Extensions;
using Infra.EventBus.RabbitMQ.Abstractions;
using Infra.EventBus.RabbitMQ.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Infra.EventBus.RabbitMQ
{
    public class RabbitMQBus : IEventBus, IDisposable
    {
        private const string RABBITMQ_EXCHANGE_NAME = "event.bus";
        private const string RABBITMQ_TYPE = "direct";
        private const string AUTOFAC_SCOPE_NAME = "event.bus";

        private readonly ILogger<RabbitMQBus> _logger;
        private readonly IRabbitMQConnection _connection;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly Env _env;

        private IModel _consumer;
        private bool _disposed;

        #region Constructor

        public RabbitMQBus(
            ILogger<RabbitMQBus> logger,
            IRabbitMQConnection connection,
            IEventBusSubscriptionsManager subsManager,
            ILifetimeScope autofac,
            IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _autofac = autofac;
            _env = new Env(config);
            _consumer = CreateConsumer();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        #endregion

        public void Publish(IntegrationEvent integrationEvent)
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_env.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
                    => _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s. ({ExceptionMessage})", integrationEvent.Id, $"{time.TotalSeconds:n1}", ex.Message));

            var eventName = integrationEvent.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId}. ({EventName})", integrationEvent.Id, eventName);

            using var channel = _connection.CreateChannel();

            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}.", integrationEvent.Id);

            channel.ExchangeDeclare(exchange: RABBITMQ_EXCHANGE_NAME, type: RABBITMQ_TYPE);

            var body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.LogTrace("Publishing event to RabbitMQ: {EventId}.", integrationEvent.Id);

                channel.BasicPublish(
                    exchange: RABBITMQ_EXCHANGE_NAME,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

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

        public void SubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}.", eventName, typeof(TDynamicIntegrationEventHandler).GetGenericTypeName());

            DoInternalSubscription(eventName);

            _subsManager.AddDynamicSubscription<TDynamicIntegrationEventHandler>(eventName);

            StartBasicConsume();
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = _subsManager.GetEventName<TIntegrationEvent>();

            _logger.LogInformation("Unsubscribing from event {EventName}.", eventName);

            _subsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void UnsubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Unsubscribing from dynamic event {EventName}.", eventName);

            _subsManager.RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(eventName);
        }

        #region Private Method

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            using var channel = _connection.CreateChannel();

            channel.QueueUnbind(
                queue: _env.QueueName,
                exchange: RABBITMQ_EXCHANGE_NAME,
                routingKey: eventName);

            if (_subsManager.IsEmpty)
                _consumer.Close();
        }

        private void DoInternalSubscription(string eventName)
        {
            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!_connection.IsConnected)
                    _connection.TryConnect();

                _consumer.QueueBind(
                    queue: _env.QueueName,
                    exchange: RABBITMQ_EXCHANGE_NAME,
                    routingKey: eventName);
            }
        }

        private IModel CreateConsumer()
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            _logger.LogTrace("Creating RabbitMQ consumer channel.");

            var channel = _connection.CreateChannel();

            channel.ExchangeDeclare(
                exchange: RABBITMQ_EXCHANGE_NAME,
                type: RABBITMQ_TYPE);

            channel.QueueDeclare(
                queue: _env.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel.");

                _consumer.Dispose();
                _consumer = CreateConsumer();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume.");

            if (_consumer != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumer);

                consumer.Received += Consumer_Received;

                _consumer.BasicConsume(
                    queue: _env.QueueName,
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
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumer.BasicAck(eventArgs.DeliveryTag, multiple: false);
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
                if (_consumer != null)
                    _consumer.Dispose();

                _subsManager.Clear();
            }

            _disposed = true;
        }

        #endregion
    }
}
