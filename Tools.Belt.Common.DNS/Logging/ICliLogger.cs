using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging
{
    public interface ICliLogger : ILogger
    {
        bool Verbose { get; set; }
        bool Quiet { get; set; }
    }
}
