using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging
{
    public interface ICliLogger<out TCategoryName> : ICliLogger, ILogger<TCategoryName>
    {
    }
}
