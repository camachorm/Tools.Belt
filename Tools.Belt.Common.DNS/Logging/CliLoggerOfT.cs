using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Logging
{
    public class CliLogger<TCategoryName> : CliLogger, ICliLogger<TCategoryName>
    {
        public CliLogger() : base() { }

        public CliLogger(IConsole console) : base(console) { }

        public CliLogger(ILogger logger) : base(logger) { }
    }
}
