using System.Net.Sockets;
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
    public class DefaultRabbitMqConnection : IRabbitMqConnection
    {
        private readonly ILogger<DefaultRabbitMqConnection> logger;
        private readonly ConnectionSettings settings;
        private readonly IConnectionFactory connectionFactory;
        private readonly object syncRoot = new();

        private IConnection connection;
        private bool disposed;

        #region Properties

        public bool IsConnected => connection is { IsOpen: true } && !disposed;

        #endregion

        #region Constructor

        public DefaultRabbitMqConnection(
            ILogger<DefaultRabbitMqConnection> logger,
            IOptions<ConnectionSettings> settings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settings = ConnectionSettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
            connectionFactory = GetConnectionFactory(this.settings);
        }

        #endregion

        public IModel CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");

            return connection.CreateModel();
        }

        public bool TryConnect()
        {
            logger.Information("RabbitMQ client is trying to connect...");

            lock (syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(settings.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
                        => logger.Warning(ex, $"RabbitMQ client could not connect after {time.TotalSeconds:n1}s. ({ex.Message})"));

                policy.Execute(() => connection = connectionFactory.CreateConnection());

                if (IsConnected)
                {
                    connection.ConnectionBlocked += OnConnectionBlocked;
                    connection.CallbackException += OnCallbackException;
                    connection.ConnectionShutdown += OnConnectionShutdown;

                    logger.Information($"RabbitMQ client acquired a persistent connection to '{connection.Endpoint.HostName}' and is subscribed to failure events.");

                    return true;
                }
                else
                {
                    logger.Critical("FATAL ERROR: RabbitMQ connections could not be created and opened.");

                    return false;
                }
            }
        }

        #region Private Method

        private static IConnectionFactory GetConnectionFactory(ConnectionSettings settings)
        {
            var factory = new ConnectionFactory()
            {
                HostName = settings.Host,
                Port = settings.Port,
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(settings.UserName))
                factory.UserName = settings.UserName;

            if (!string.IsNullOrEmpty(settings.Password))
                factory.Password = settings.Password;

            return factory;
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (disposed)
                return;

            logger.Warning("A RabbitMQ connection is blocked. Trying to re-connect...");

            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (disposed)
                return;

            logger.Warning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (disposed)
                return;

            logger.Warning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }

        #endregion

        #region Dispose

        ~DefaultRabbitMqConnection() => Dispose(false);

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
                try
                {
                    connection.Dispose();
                }
                catch (IOException ex)
                {
                    logger.Critical($"{ex}");
                }
            }

            disposed = true;
        }

        #endregion
    }
}
