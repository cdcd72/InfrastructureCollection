using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
    public class RabbitMqBus : IEventBus, IDisposable
    {
        private const string RABBITMQ_EXCHANGE_NAME = "event.bus";
        private const string RABBITMQ_TYPE = "direct";
        private const string AUTOFAC_SCOPE_NAME = "event.bus";

        private readonly ILogger<RabbitMqBus> logger;
        private readonly Settings settings;
        private readonly IRabbitMqConnection connection;
        private readonly IEventBusSubscriptionsManager subsManager;
        private readonly ILifetimeScope autofac;

        private IModel consumer;
        private bool disposed;

        #region Constructor

        public RabbitMqBus(
            ILogger<RabbitMqBus> logger,
            IOptions<Settings> settings,
            IRabbitMqConnection connection,
            IEventBusSubscriptionsManager subsManager,
            ILifetimeScope autofac)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            this.autofac = autofac;
            consumer = CreateConsumer();
            this.subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        #endregion

        public void Publish(IntegrationEvent integrationEvent)
        {
            if (!connection.IsConnected)
                connection.TryConnect();

            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(settings.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
                    => logger.Warning(ex, $"Could not publish event: {integrationEvent.Id} after {time.TotalSeconds:n1}s. ({ex.Message})"));

            var eventName = integrationEvent.GetType().Name;

            logger.Trace($"Creating RabbitMQ channel to publish event: {integrationEvent.Id}. ({eventName})");

            using var channel = connection.CreateChannel();

            logger.Trace($"Declaring RabbitMQ exchange to publish event: {integrationEvent.Id}.");

            channel.ExchangeDeclare(exchange: RABBITMQ_EXCHANGE_NAME, type: RABBITMQ_TYPE);

            var body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                logger.Trace($"Publishing event to RabbitMQ: {integrationEvent.Id}.");

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
            var eventName = subsManager.GetEventName<TIntegrationEvent>();

            DoInternalSubscription(eventName);

            logger.Information($"Subscribing to event {eventName} with {typeof(TIntegrationEventHandler).GetGenericTypeName()}.");

            subsManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();

            StartBasicConsume();
        }

        public void SubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            logger.Information($"Subscribing to dynamic event {eventName} with {typeof(TDynamicIntegrationEventHandler).GetGenericTypeName()}.");

            DoInternalSubscription(eventName);

            subsManager.AddDynamicSubscription<TDynamicIntegrationEventHandler>(eventName);

            StartBasicConsume();
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = subsManager.GetEventName<TIntegrationEvent>();

            logger.Information($"Unsubscribing from event {eventName}.");

            subsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void UnsubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            logger.Information($"Unsubscribing from dynamic event {eventName}.");

            subsManager.RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(eventName);
        }

        #region Private Method

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!connection.IsConnected)
                connection.TryConnect();

            using var channel = connection.CreateChannel();

            channel.QueueUnbind(
                queue: settings.QueueName,
                exchange: RABBITMQ_EXCHANGE_NAME,
                routingKey: eventName);

            if (subsManager.IsEmpty)
                consumer.Close();
        }

        private void DoInternalSubscription(string eventName)
        {
            if (!subsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!connection.IsConnected)
                    connection.TryConnect();

                consumer.QueueBind(
                    queue: settings.QueueName,
                    exchange: RABBITMQ_EXCHANGE_NAME,
                    routingKey: eventName);
            }
        }

        private IModel CreateConsumer()
        {
            if (!connection.IsConnected)
                connection.TryConnect();

            logger.Trace("Creating RabbitMQ consumer channel.");

            var channel = connection.CreateChannel();

            channel.ExchangeDeclare(
                exchange: RABBITMQ_EXCHANGE_NAME,
                type: RABBITMQ_TYPE);

            channel.QueueDeclare(
                queue: settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.CallbackException += (_, ea) =>
            {
                logger.Warning(ea.Exception, "Recreating RabbitMQ consumer channel.");

                consumer.Dispose();
                consumer = CreateConsumer();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            logger.Trace("Starting RabbitMQ basic consume.");

            if (consumer != null)
            {
                var basicConsumer = new AsyncEventingBasicConsumer(consumer);

                basicConsumer.Received += Consumer_Received;

                consumer.BasicConsume(
                    queue: settings.QueueName,
                    autoAck: false,
                    consumer: basicConsumer);
            }
            else
            {
                logger.Error("StartBasicConsume can't call on _consumerChannel == null .");
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
                logger.Warning(ex, $"----- ERROR Processing message \"{message}\"");
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX).
            // For more information see: https://www.rabbitmq.com/dlx.html
            consumer.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            logger.Trace($"Processing RabbitMQ event: {eventName}");

            if (subsManager.HasSubscriptionsForEvent(eventName))
            {
                await using var scope = autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);

                var subscriptions = subsManager.GetHandlersForEvent(eventName);

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

                        var eventType = subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
            else
            {
                logger.Warning($"No subscription for RabbitMQ event: {eventName}");
            }
        }

        #endregion

        #region Dispose

        ~RabbitMqBus() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                consumer?.Dispose();
                subsManager.Clear();
            }

            disposed = true;
        }

        #endregion
    }
}
