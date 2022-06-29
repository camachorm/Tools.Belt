using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Abstractions.Configuration
{
    public abstract class ConfigurationProviderBase : IConfigurationProvider
    {
        protected ConfigurationProviderBase(ILogger logger, IConfigurationService service)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Service = service ?? throw new ArgumentNullException(nameof(service));
            ProviderCount++;
        }

        protected static int ProviderCount { get; private set; }

        protected IConfigurationService Service { get; }

        protected virtual ILogger Logger { get; }

        public abstract string this[string key] { get; set; }

        public virtual string ProviderId => $"{GetType().FullName}.{ProviderCount}";

        public abstract IEnumerable<string> List { get; }

        public abstract IEnumerable<string> Keys { get; }

        public virtual string ReadKey(string key)
        {
            return this[key];
        }

        public virtual async Task<string> ReadKeyAsync(string key)
        {
            return await new Task<string>(() => this[key]).ConfigureAwait(false);
        }
    }
}