using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;

namespace Tools.Belt.Common.Logging.Cli
{
    public class CliLogger : ICliLogger
    {
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        protected readonly IConsole _console;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
        protected readonly ConsoleReporter _reporter;
        protected readonly ILogger _logger;
        private bool _verbose;
        private bool _quiet;

        public CliLogger() : this(PhysicalConsole.Singleton) { }

        public CliLogger(IConsole console)
        {
            _console = console;
            _reporter = new ConsoleReporter(_console);
        }

        public CliLogger(ILogger logger)
        {
            _logger = logger;
        }

        public bool Verbose
        {
            get => (_reporter?.IsVerbose).GetValueOrDefault(_verbose);
            set
            {
                _verbose = value;
                if (_reporter != null) _reporter.IsVerbose = value;
            }
        }

        public bool Quiet
        {
            get => (_reporter?.IsQuiet).GetValueOrDefault(_quiet);
            set
            {
                _quiet = value;
                if (_reporter != null) _reporter.IsQuiet = value;
            }
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            string message = $"[{DateTime.Now:G}] {formatter(state, exception)}";

            _logger?.Log(logLevel, eventId, state, exception, formatter);

            if (_reporter == null) return;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _reporter.Verbose(message);
                    break;
                case LogLevel.Debug:
                    _reporter.Verbose(message);
                    break;
                case LogLevel.Information:
                    _reporter.Output(message);
                    break;
                case LogLevel.Warning:
                    _reporter.Warn(message);
                    break;
                case LogLevel.Error:
                    _reporter.Error(message);
                    break;
                case LogLevel.Critical:
                    _reporter.Error(message);
                    break;
                default:
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return !Quiet && Verbose;
                case LogLevel.Debug:
                    return !Quiet && Verbose;
                case LogLevel.Information:
                    return !Quiet;
                case LogLevel.Warning:
                    return !Quiet;
                case LogLevel.Error:
                    return true;
                case LogLevel.Critical:
                    return true;
                case LogLevel.None:
                    return true;
                default:
                    throw new NotImplementedException($"Log level {logLevel} not implemented.");
            }
        }

        public IDisposable BeginScope<TState>(TState state) => default;
    }
}
