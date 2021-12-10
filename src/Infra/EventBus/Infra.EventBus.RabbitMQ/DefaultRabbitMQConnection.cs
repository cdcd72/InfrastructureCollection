using System;
using System.IO;
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
    public class DefaultRabbitMQConnection : IRabbitMQConnection
    {
        private readonly ILogger<DefaultRabbitMQConnection> _logger;
        private readonly ConnectionSettings _settings;
        private readonly IConnectionFactory _connectionFactory;
        private readonly object _syncRoot = new();

        private IConnection _connection;
        private bool _disposed;

        #region Properties

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        #endregion

        #region Constructor

        public DefaultRabbitMQConnection(
            ILogger<DefaultRabbitMQConnection> logger,
            IOptions<ConnectionSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = ConnectionSettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
            _connectionFactory = GetConnectionFactory(_settings);
        }

        #endregion

        public IModel CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");

            return _connection.CreateModel();
        }

        public bool TryConnect()
        {
            _logger.Information("RabbitMQ client is trying to connect...");

            lock (_syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_settings.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
                        => _logger.Warning(ex, $"RabbitMQ client could not connect after {time.TotalSeconds:n1}s. ({ex.Message})"));

                policy.Execute(() => _connection = _connectionFactory.CreateConnection());

                if (IsConnected)
                {
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionShutdown += OnConnectionShutdown;

                    _logger.Information($"RabbitMQ client acquired a persistent connection to '{_connection.Endpoint.HostName}' and is subscribed to failure events.");

                    return true;
                }
                else
                {
                    _logger.Critical("FATAL ERROR: RabbitMQ connections could not be created and opened.");

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
            if (_disposed)
                return;

            _logger.Warning("A RabbitMQ connection is blocked. Trying to re-connect...");

            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed)
                return;

            _logger.Warning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed)
                return;

            _logger.Warning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }

        #endregion

        #region Dispose

        ~DefaultRabbitMQConnection() => Dispose(false);

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
                    _logger.Critical($"{ex}");
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
