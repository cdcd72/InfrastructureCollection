using System;
using System.IO;
using System.Net.Sockets;
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
    public class DefaultRabbitMQConnection : IRabbitMQConnection
    {
        private readonly ILogger<DefaultRabbitMQConnection> _logger;
        private readonly Env _env;
        private readonly IConnectionFactory _connectionFactory;
        private readonly object _syncRoot = new();

        private IConnection _connection;
        private bool _disposed;

        #region Properties

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        #endregion

        #region Constructor

        public DefaultRabbitMQConnection(ILogger<DefaultRabbitMQConnection> logger, IConfiguration config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = new Env(config);
            _connectionFactory = GetConnectionFactory(_env);
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
            _logger.LogInformation("RabbitMQ client is trying to connect...");

            lock (_syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_env.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
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

        private static IConnectionFactory GetConnectionFactory(Env env)
        {
            var factory = new ConnectionFactory()
            {
                HostName = env.Host,
                Port = env.Port,
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(env.UserName))
                factory.UserName = env.UserName;

            if (!string.IsNullOrEmpty(env.Password))
                factory.Password = env.Password;

            return factory;
        }

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
                    _logger.LogCritical($"{ex}");
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
