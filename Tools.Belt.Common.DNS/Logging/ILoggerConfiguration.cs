using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging
{
    public interface ILoggerConfiguration
    {
        LogLevel LogLevel { get; set; }
    }
}