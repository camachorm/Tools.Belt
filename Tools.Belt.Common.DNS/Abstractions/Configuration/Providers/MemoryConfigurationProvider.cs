using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Abstractions.Configuration.Providers
{
    public class MemoryConfigurationProvider : ConfigurationProviderBase, IMemoryConfigurationProvider
    {
        private readonly ConcurrentDictionary<string, string> _dictionary;

        public MemoryConfigurationProvider(ILogger logger, IConfigurationService service)
            : this(logger, service, new ConcurrentDictionary<string, string>())
        {
        }

        public MemoryConfigurationProvider(
            ILogger logger,
            IConfigurationService service,
            IEnumerable<KeyValuePair<string, string>> configurationEntries)
            : this(logger, service, configurationEntries.ToDictionary(tuple => tuple.Key, tuple => tuple.Value))
        {
        }

        public MemoryConfigurationProvider(ILogger logger, IConfigurationService service,
            IEnumerable<Tuple<string, string>> configurationEntries)
            : this(logger, service, configurationEntries.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2))
        {
        }

        public MemoryConfigurationProvider(ILogger logger, IConfigurationService service,
            Dictionary<string, string> configurationEntries)
            : this(logger, service, new ConcurrentDictionary<string, string>(configurationEntries))
        {
        }

        public MemoryConfigurationProvider(ILogger logger, IConfigurationService service,
            ConcurrentDictionary<string, string> configurationEntries)
            : base(logger, service)
        {
            _dictionary = configurationEntries;
        }

        public override string this[string key]
        {
            get => _dictionary[key];
            set => _dictionary.AddOrUpdate(key, value, (k, v) => value);
        }

        public override IEnumerable<string> List =>
            _dictionary.Select(pair => $"{pair.Key}/{pair.Value.ToPrintableSecret(40)}");

        public override IEnumerable<string> Keys => _dictionary.Keys;
    }
}