using System;
using System.IO;
using System.Net.Sockets;
using Infra.EventBus.RabbitMQ.Abstractions;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Infra.EventBus.RabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private readonly int _retryCount;
        private readonly object _syncRoot = new();

        private IConnection _connection;
        private bool _disposed;

        #region Properties

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        #endregion

        #region Constructor

        public DefaultRabbitMQPersistentConnection(
            IConnectionFactory connectionFactory,
            ILogger<DefaultRabbitMQPersistentConnection> logger,
            int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }

        #endregion

        public IModel CreateModel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");

            return _connection.CreateModel();
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ client is trying to connect...");

            lock (_syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
                        => _logger.LogWarning(ex, "RabbitMQ client could not connect after {TimeOut}s. ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message));

                policy.Execute(() => _connection = _connectionFactory.CreateConnection());

                if (IsConnected)
                {
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionShutdown += OnConnectionShutdown;

                    _logger.LogInformation("RabbitMQ client acquired a persistent connection to '{HostName}' and is subscribed to failure events.", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened.");

                    return false;
                }
            }
        }

        #region Private Method

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed)
                return;

            _logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");

            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed)
                return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed)
                return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }

        #endregion

        #region Dispose

        ~DefaultRabbitMQPersistentConnection() => Dispose(false);

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
                try
                {
                    _connection.Dispose();
                }
                catch (IOException ex)
                {
                    _logger.LogCritical($"{ex}");
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
