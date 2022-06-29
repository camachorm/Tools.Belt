// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging.Trace
{
    [ExcludeFromCodeCoverage]
    public static class TraceLoggerExtensions
    {
        private static TraceLoggerProvider _provider;

        public static ILoggerFactory AddTraceLogger(this ILoggerFactory source, ITraceLoggerConfiguration configuration)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            _provider = new TraceLoggerProvider(configuration);
            source.AddProvider(_provider);

            return source;
        }

        public static ILoggerFactory AddTraceLogger(
            this ILoggerFactory source,
            Action<ITraceLoggerConfiguration> configure)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            TraceLoggerConfiguration configuration = new TraceLoggerConfiguration();

            configure(configuration);

            return source.AddTraceLogger(configuration);
        }

        public static ILoggerFactory AddTraceLogger(this ILoggerFactory source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.AddTraceLogger(new TraceLoggerConfiguration());
        }
    }
}