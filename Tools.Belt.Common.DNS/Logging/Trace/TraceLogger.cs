using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Mocks;

namespace Tools.Belt.Common.Logging.Trace
{
    [ExcludeFromCodeCoverage]
    public class TraceLogger<T> : TraceLogger, ILogger<T>
    {
        public TraceLogger(string categoryName, ITraceLoggerConfiguration configuration) : base(categoryName,
            configuration)
        {
        }
    }

    [ExcludeFromCodeCoverage]
    public class TraceLogger : ILogger
    {
        public TraceLogger(string categoryName, ITraceLoggerConfiguration configuration)
        {
            CategoryName = categoryName;
            Configuration = configuration;
        }

        public string CategoryName { get; }
        public ITraceLoggerConfiguration Configuration { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            string message = formatter.Invoke(state, exception);

            System.Diagnostics.Trace.TraceInformation(message);
            Debug.WriteLine(message, CategoryName);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new DisposableScope();
        }
    }
}