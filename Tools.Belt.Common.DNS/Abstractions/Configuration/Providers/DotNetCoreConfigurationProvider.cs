using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Abstractions.Configuration.Providers
{
    public class DotNetCoreConfigurationProvider : ConfigurationProviderBase, IMemoryConfigurationProvider
    {
        private readonly IConfiguration _configuration;

        public DotNetCoreConfigurationProvider(ILogger logger, IConfigurationService service,
            IConfiguration configuration) : base(logger, service)
        {
            _configuration = configuration;
        }

        public override string this[string key]
        {
            get => _configuration[key];
            set => _configuration[key] = value;
        }

        public override IEnumerable<string> List => _configuration.AllConfig()
            .Select(pair => $"{ProviderId}/{pair.Key}/{pair.Value.ToPrintableSecret(40)}");

        public override IEnumerable<string> Keys => _configuration.AllConfig().Select((kvp, i) => kvp.Key);
    }
}