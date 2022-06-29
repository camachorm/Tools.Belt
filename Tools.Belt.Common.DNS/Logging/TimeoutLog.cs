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

        public void Dispose()
        {
            if (_sw.Elapsed >= Timeout)
            {
                switch (Level)
                {
                    case SeverityLevel.Verbose:
                        _logger?.LogTrace(_timeoutMessage);
                        break;
                    case SeverityLevel.Information:
                        _logger?.LogInformation(_timeoutMessage);
                        break;
                    case SeverityLevel.Warning:
                        _logger?.LogWarning(_timeoutMessage);
                        break;
                    case SeverityLevel.Error:
                        _logger?.LogError(_timeoutMessage);
                        break;
                    case SeverityLevel.Critical:
                        _logger?.LogCritical(_timeoutMessage);
                        break;
                }
            }
        }
    }
}