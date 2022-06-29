using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging.Cli
{
    public interface ICliLogger<out TCategoryName> : ICliLogger, ILogger<TCategoryName>
    {
    }
}
