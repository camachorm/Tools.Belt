using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging.Trace
{
    [ExcludeFromCodeCoverage]
    public class TraceLoggerProvider : ILoggerProvider
    {
        private readonly ITraceLoggerConfiguration _configuration;

        private readonly ConcurrentDictionary<string, TraceLogger> _loggers =
            new ConcurrentDictionary<string, TraceLogger>();

        public TraceLoggerProvider(ITraceLoggerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ILogger CreateLogger(string categoryName)
        {
            lock (_loggers)
            {
                return _loggers.GetOrAdd(categoryName, name => new TraceLogger(name, _configuration));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) _loggers.Clear();
        }
    }
}