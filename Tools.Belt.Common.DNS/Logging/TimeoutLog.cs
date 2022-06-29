using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Tools.Belt.Common.Logging
{
    public sealed class TimeoutLog : IDisposable
    {
        private readonly Stopwatch _sw;
        private readonly ILogger _logger;
        private readonly string _timeoutMessage;

        /// <summary>
        /// On disposal, check if the timeout exceeds the time after its creation and if it does, logs a message.
        /// </summary>
        /// <param name="timeout">Timeout time.</param>
        /// <param name="logger">Logger to log with.</param>
        /// <param name="timeoutMessage">Timeout message.</param>
        /// <param name="severityLevel">Severity level.</param>
        public TimeoutLog(
            TimeSpan timeout,
            ILogger logger,
            string timeoutMessage,
            SeverityLevel severityLevel = SeverityLevel.Warning)
        {
            _sw = Stopwatch.StartNew();
            _logger = logger;
            _timeoutMessage = timeoutMessage;
            Timeout = timeout;
            Level = severityLevel;
        }

        public SeverityLevel Level { get; set; }
        public TimeSpan Timeout { get; set; }
        public TimeSpan Elapsed => _sw.Elapsed;

        private LogLevel LoggingLevel
        {
            get
            {
                switch (Level)
                {
                    case SeverityLevel.Verbose:
                        return LogLevel.Trace;
                    case SeverityLevel.Information:
                        return LogLevel.Information;
                    case SeverityLevel.Warning:
                        return LogLevel.Warning;
                    case SeverityLevel.Error:
                        return LogLevel.Error;
                    case SeverityLevel.Critical:
                        return LogLevel.Critical;
                    default:
                        return LogLevel.Information;
                }
            }
        }

        public void Dispose()
        {
            if (_sw.Elapsed >= Timeout)
            {
                _logger?.Log(LoggingLevel, _timeoutMessage);
            }
        }
    }
}