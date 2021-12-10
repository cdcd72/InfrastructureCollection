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
using Infra.Core.Extensions;
using Infra.EventBus.RabbitMQ.Abstractions;
using Infra.EventBus.RabbitMQ.Configuration;
using Infra.EventBus.RabbitMQ.Configuration.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly Settings _settings;
        private readonly IRabbitMQConnection _connection;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;

        private IModel _consumer;
        private bool _disposed;

        #region Constructor

        public RabbitMQBus(
            ILogger<RabbitMQBus> logger,
            IOptions<Settings> settings,
            IRabbitMQConnection connection,
            IEventBusSubscriptionsManager subsManager,
            ILifetimeScope autofac)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _settings = settings.Value;

            if (!SettingsValidator.TryValidate(_settings, out var validationException))
                throw validationException;

            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _autofac = autofac;
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
                .WaitAndRetry(_settings.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
                    => _logger.Warning(ex, $"Could not publish event: {integrationEvent.Id} after {time.TotalSeconds:n1}s. ({ex.Message})"));

            var eventName = integrationEvent.GetType().Name;

            _logger.Trace($"Creating RabbitMQ channel to publish event: {integrationEvent.Id}. ({eventName})");

            using var channel = _connection.CreateChannel();

            _logger.Trace($"Declaring RabbitMQ exchange to publish event: {integrationEvent.Id}.");

            channel.ExchangeDeclare(exchange: RABBITMQ_EXCHANGE_NAME, type: RABBITMQ_TYPE);

            var body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.Trace($"Publishing event to RabbitMQ: {integrationEvent.Id}.");

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

            _logger.Information($"Subscribing to event {eventName} with {typeof(TIntegrationEventHandler).GetGenericTypeName()}.");

            _subsManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();

            StartBasicConsume();
        }

        public void SubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            _logger.Information($"Subscribing to dynamic event {eventName} with {typeof(TDynamicIntegrationEventHandler).GetGenericTypeName()}.");

            DoInternalSubscription(eventName);

            _subsManager.AddDynamicSubscription<TDynamicIntegrationEventHandler>(eventName);

            StartBasicConsume();
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = _subsManager.GetEventName<TIntegrationEvent>();

            _logger.Information($"Unsubscribing from event {eventName}.");

            _subsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void UnsubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            _logger.Information($"Unsubscribing from dynamic event {eventName}.");

            _subsManager.RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(eventName);
        }

        #region Private Method

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            using var channel = _connection.CreateChannel();

            channel.QueueUnbind(
                queue: _settings.QueueName,
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
                    queue: _settings.QueueName,
                    exchange: RABBITMQ_EXCHANGE_NAME,
                    routingKey: eventName);
            }
        }

        private IModel CreateConsumer()
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            _logger.Trace("Creating RabbitMQ consumer channel.");

            var channel = _connection.CreateChannel();

            channel.ExchangeDeclare(
                exchange: RABBITMQ_EXCHANGE_NAME,
                type: RABBITMQ_TYPE);

            channel.QueueDeclare(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.Warning(ea.Exception, "Recreating RabbitMQ consumer channel.");

                _consumer.Dispose();
                _consumer = CreateConsumer();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            _logger.Trace("Starting RabbitMQ basic consume.");

            if (_consumer != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumer);

                consumer.Received += Consumer_Received;

                _consumer.BasicConsume(
                    queue: _settings.QueueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.Error("StartBasicConsume can't call on _consumerChannel == null .");
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
                _logger.Warning(ex, $"----- ERROR Processing message \"{message}\"");
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX).
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumer.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.Trace($"Processing RabbitMQ event: {eventName}");

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
                _logger.Warning($"No subscription for RabbitMQ event: {eventName}");
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
