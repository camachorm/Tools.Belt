using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging.Trace
{
    [ExcludeFromCodeCoverage]
    public class TraceLoggerConfiguration : ITraceLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    }
}