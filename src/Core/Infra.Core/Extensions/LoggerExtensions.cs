using Microsoft.Extensions.Logging;
using System;

namespace Infra.Core.Extensions
{
    public static class LoggerExtensions
    {
        private static Action<ILogger, string, Exception> TraceExecuted => LoggerMessage.Define<string>(
                LogLevel.Trace,
                0,
                "{Message}"
            );

        private static Action<ILogger, string, Exception> DebugExecuted => LoggerMessage.Define<string>(
                LogLevel.Debug,
                1,
                "{Message}"
            );

        private static Action<ILogger, string, Exception> InformationExecuted => LoggerMessage.Define<string>(
                LogLevel.Information,
                2,
                "{Message}"
            );

        private static Action<ILogger, string, Exception> WarningExecuted => LoggerMessage.Define<string>(
                LogLevel.Warning,
                3,
                "{Message}"
            );

        private static Action<ILogger, string, Exception> ErrorExecuted => LoggerMessage.Define<string>(
                LogLevel.Error,
                4,
                "{Message}"
            );

        private static Action<ILogger, string, Exception> CriticalExecuted => LoggerMessage.Define<string>(
                LogLevel.Critical,
                5,
                "{Message}"
            );

        #region Trace

        public static void Trace(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Trace, null, message);

        public static void Trace(this ILogger logger, Exception ex, string message)
            => LogMessage(logger, LogLevel.Trace, ex, message);

        #endregion

        #region Debug

        public static void Debug(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Debug, null, message);

        public static void Debug(this ILogger logger, Exception ex, string message)
            => LogMessage(logger, LogLevel.Debug, ex, message);

        #endregion

        #region Information

        public static void Information(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Information, null, message);

        public static void Information(this ILogger logger, Exception ex, string message)
            => LogMessage(logger, LogLevel.Information, ex, message);

        #endregion

        #region Warning

        public static void Warning(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Warning, null, message);

        public static void Warning(this ILogger logger, Exception ex, string message)
            => LogMessage(logger, LogLevel.Warning, ex, message);

        #endregion

        #region Error

        public static void Error(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Error, null, message);

        public static void Error(this ILogger logger, Exception ex, string message)
            => LogMessage(logger, LogLevel.Error, ex, message);

        #endregion

        #region Critical

        public static void Critical(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Critical,null, message);

        public static void Critical(this ILogger logger, Exception ex, string message)
            => LogMessage(logger, LogLevel.Critical, ex, message);

        #endregion

        #region Private Method

        private static void LogMessage(ILogger logger, LogLevel logLevel, Exception ex, string message)
        {
            message = message.PreventLogForging();

            switch (logLevel)
            {
                case LogLevel.Trace:
                    TraceExecuted(logger, message, ex);
                    break;
                case LogLevel.Debug:
                    DebugExecuted(logger, message, ex);
                    break;
                case LogLevel.Information:
                    InformationExecuted(logger, message, ex);
                    break;
                case LogLevel.Warning:
                    WarningExecuted(logger, message, ex);
                    break;
                case LogLevel.Error:
                    ErrorExecuted(logger, message, ex);
                    break;
                case LogLevel.Critical:
                    CriticalExecuted(logger, message, ex);
                    break;
                case LogLevel.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        #endregion
    }
}
