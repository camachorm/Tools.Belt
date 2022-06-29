using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Abstractions.Configuration.Providers
{
    public class EnvironmentVariablesConfigurationProvider : ConfigurationProviderBase, IMemoryConfigurationProvider
    {
        public EnvironmentVariablesConfigurationProvider(ILogger logger, IConfigurationService service) : base(logger,
            service)
        {
        }

        public override string this[string key]
        {
            get => System.Environment.GetEnvironmentVariable(key);
            set => System.Environment.SetEnvironmentVariable(key, value);
        }

        public override IEnumerable<string> List => System.Environment.GetEnvironmentVariables().Keys.Cast<string>()
            .Select(
                (key, i) =>
                    $"{key}: {System.Environment.GetEnvironmentVariable(key).ToPrintableSecret(40)}");

        public override IEnumerable<string> Keys => System.Environment.GetEnvironmentVariables().Keys.Cast<string>();
    }
}