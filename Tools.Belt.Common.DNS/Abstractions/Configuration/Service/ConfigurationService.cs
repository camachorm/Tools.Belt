using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Abstractions.Configuration.Service
{
    public sealed class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(IConfigurationBuilder builder)
        {
            Builder = builder;
        }

        private List<IConfigurationProvider> ConfigurationProviders { get; } = new List<IConfigurationProvider>();

        public IConfigurationBuilder Builder { get; }

        public string this[string key]
        {
            get => ConfigurationProviders.FirstOrDefault(provider => provider.Keys.Contains(key))?[key];
            set => ConfigurationProviders.First(provider => provider.Keys.Contains(key))[key] = value;
        }

        public IEnumerable<string> List =>
            ConfigurationProviders.SelectMany(provider => provider.List.Select(s => $"{provider.ProviderId}:: {s}"));

        public IEnumerable<string> Keys =>
            ConfigurationProviders.SelectMany(provider => provider.Keys.Select(s => $"{provider.ProviderId}: {s}"));

        public IEnumerable<string> ProviderList => ConfigurationProviders.Select(provider => provider.ProviderId);

        public IConfigurationService RegisterProvider(IConfigurationBuilder builder, IConfigurationProvider newProvider)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (newProvider == null) throw new ArgumentNullException(nameof(newProvider));

            if (!builder.Options.AllowDuplicateKeysInDifferentProviders)
            {
                bool hasDuplicates = false;
                List<string> duplicatesFullList = new List<string>();

                foreach (IConfigurationProvider existingProvider in ConfigurationProviders)
                {
                    List<string> duplicates = newProvider.Keys.Where(s => existingProvider.Keys.Contains(s))
                        .Select(s => $"{existingProvider.ProviderId}: {s}").ToList();
                    if (!duplicates.Any()) continue;
                    hasDuplicates = true;
                    duplicatesFullList.AddRange(duplicates);
                }

                if (hasDuplicates)
                    throw new NotSupportedException(
                        $"While adding {newProvider.ProviderId}, Duplicate keys found:{System.Environment.NewLine}{duplicatesFullList.Flatten().Indent(1)}");
            }

            ConfigurationProviders.Add(newProvider);
            return this;
        }

        public string ReadKey(string key)
        {
            return this[key];
        }

        public async Task<string> ReadKeyAsync(string key)
        {
            return await Task.FromResult(this[key]).ConfigureAwait(false);
        }
    }
}