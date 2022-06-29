using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        /// <summary>
        ///     Reads a configuration entry with key <see cref="configName" /> from <see cref="source" />  and censors its content,
        ///     displaying a maximum of <see cref="maxChars" />
        ///     chars at the start and end of the text.
        /// </summary>
        /// <param name="source">The configuration instance to use</param>
        /// <param name="configName">The key identifier of the desired configuration entry</param>
        /// <param name="maxChars">The maximum number of characters to display at either end of the string</param>
        /// <param name="filters">
        ///     The list of filters to apply. If none is provided (e.g. filters = null), then default filter is
        ///     applied (e.g. filters = new[] {"KeyVaultIndexedConfiguration"})
        /// </param>
        /// <returns>The censored value of the provided <see cref="configName" /></returns>
        public static string ReadConfigAndCensorSecrets(
            this IConfigurationService source,
            string configName,
            int maxChars = 4,
            IEnumerable<string> filters = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (configName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(configName));

            filters ??= new[] {"KeyVaultIndexedConfiguration"};

            string result = source[configName.FilterOutIConfigurationProvider()];

            return filters.Any(filter =>
                configName.StartsWith(filter, StringComparison.OrdinalIgnoreCase) ||
                configName.EndsWith(filter, StringComparison.OrdinalIgnoreCase))
                ? result.ToPrintableSecret(30, maxChars)
                : result;
        }

        public static IList<KeyValuePair<string, string>> AllConfig(this IConfiguration configuration)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (configuration == null) return result;

            result.AddRange(configuration.AsEnumerable()
                .Select(item => new KeyValuePair<string, string>(item.Key, item.Value)));

            //foreach (IConfigurationSection section in configuration.GetChildren()) result.AddRange(AllConfig((IConfiguration)section));

            return result;
        }

        public static IList<Tuple<string, string, string>> AllConfig(this IConfigurationSection section)
        {
            if (section == null) return new List<Tuple<string, string, string>>();

            List<Tuple<string, string, string>> result = new List<Tuple<string, string, string>>
                {new Tuple<string, string, string>(section.Key, section.Value, section.Path)};

            IEnumerable<IConfigurationSection> children = section.GetChildren();

            foreach (IConfigurationSection child in children) result.AddRange(child.AllConfig());

            return result;
        }

        /// <summary>
        ///     Reads all the currently loaded configuration entries from <see cref="source" /> and censors its content based on
        ///     the provided <see cref="filters" />, displaying a maximum of <see cref="maxChars" />
        ///     chars on the censured entries at the start and end of the text.
        /// </summary>
        /// <param name="source">The configuration instance to use</param>
        /// <param name="maxChars">The maximum number of characters to display at either end of the censored string</param>
        /// <param name="filters">
        ///     The list of filters to apply. If none is provided (e.g. filters = null), then default filter is
        ///     applied (e.g. filters = new[] {"KeyVaultIndexedConfiguration"})
        /// </param>
        /// <returns>
        ///     A new JObject with multiple Properties with values being censored/uncensored depending on the provided
        ///     <see cref="filters" />
        /// </returns>
        public static JObject ToJObject(this IConfigurationService source, int maxChars = 4,
            IEnumerable<string> filters = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            JObject configuration = new JObject();
            IEnumerable<string> keys = source.Keys;
            foreach (string key in keys)
                // ReSharper disable PossibleMultipleEnumeration
                configuration.Add(key, source.ReadConfigAndCensorSecrets(key, maxChars, filters));
            // ReSharper restore PossibleMultipleEnumeration

            return configuration;
        }
    }
}