using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration.Providers;

namespace Tools.Belt.Common.Abstractions.Configuration.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Configures desired behavior should there be duplicate keys in 2 different
        ///     <see cref="Configuration.IConfigurationProvider" /> instances
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance.</param>
        /// <param name="enable">True to allow different providers to use the same keys, false otherwise</param>
        /// <returns>The configured <see cref="IConfigurationBuilder" /> instance.</returns>
        public static IConfigurationBuilder ConfigureSupportForDuplicateKeysInDifferentProviders(
            this IConfigurationBuilder builder,
            bool enable = false)
        {
            AssertBuilderNotNull(builder);
            builder.Options.AllowDuplicateKeysInDifferentProviders = enable;
            return builder;
        }

        /// <summary>
        ///     Adds a clean <see cref="IMemoryConfigurationProvider" /> instance to the <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(
            this IConfigurationBuilder builder)
        {
            AssertBuilderNotNull(builder);
            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IMemoryConfigurationProvider" /> instance with the provided entries to the
        ///     <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <param name="configurationEntries">
        ///     The list of configuration entries to initialize the
        ///     <see cref="IMemoryConfigurationProvider" /> with
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(
            this IConfigurationBuilder builder,
            IEnumerable<Tuple<string, string>> configurationEntries)
        {
            AssertBuilderNotNull(builder);
            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service,
                    configurationEntries));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IMemoryConfigurationProvider" /> instance with the provided entries to the
        ///     <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <param name="configurationEntries">
        ///     The list of configuration entries to initialize the
        ///     <see cref="IMemoryConfigurationProvider" /> with
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(
            this IConfigurationBuilder builder,
            IEnumerable<KeyValuePair<string, string>> configurationEntries)
        {
            AssertBuilderNotNull(builder);
            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service,
                    configurationEntries));

            return builder;
        }


        /// <summary>
        ///     Adds a <see cref="IMemoryConfigurationProvider" /> instance with the provided entries to the
        ///     <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <param name="configurationEntries">
        ///     The list of configuration entries to initialize the
        ///     <see cref="IMemoryConfigurationProvider" /> with
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(
            this IConfigurationBuilder builder,
            Dictionary<string, string> configurationEntries)
        {
            AssertBuilderNotNull(builder);
            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service,
                    configurationEntries));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IMemoryConfigurationProvider" /> instance with the provided entries to the
        ///     <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <param name="configurationEntries">
        ///     The list of configuration entries to initialize the
        ///     <see cref="IMemoryConfigurationProvider" /> with
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(
            this IConfigurationBuilder builder,
            ConcurrentDictionary<string, string> configurationEntries)
        {
            AssertBuilderNotNull(builder);
            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service,
                    configurationEntries));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IMemoryConfigurationProvider" /> instance with the provided entries to the
        ///     <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <param name="configurationEntries">
        ///     The list of configuration entries to initialize the
        ///     <see cref="IMemoryConfigurationProvider" /> with
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(
            this IConfigurationBuilder builder,
            IDictionary configurationEntries)
        {
            AssertBuilderNotNull(builder);
            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service,
                    configurationEntries.Keys.Cast<string>().Select(key =>
                        new KeyValuePair<string, string>(key, configurationEntries[key].ToString()))));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IMemoryConfigurationProvider" /> instance with the provided entries to the
        ///     <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <param name="configurationEntries">
        ///     The list of configuration entries to initialize the
        ///     <see cref="IMemoryConfigurationProvider" /> with
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(
            this IConfigurationBuilder builder,
            NameValueCollection configurationEntries)
        {
            AssertBuilderNotNull(builder);

            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service,
                    configurationEntries.AllKeys.ToDictionary(k => k, k => configurationEntries[k])));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IMemoryConfigurationProvider" /> instance with the provided entries to the
        ///     <see cref="IConfigurationService" />
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder" /> instance</param>
        /// <param name="configurationEntries">
        ///     The list of configuration entries to initialize the
        ///     <see cref="IMemoryConfigurationProvider" /> with
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder" /> instance</returns>
        public static IConfigurationBuilder AddInMemoryConfiguration(this IConfigurationBuilder builder,
            IConfiguration configurationEntries)
        {
            AssertBuilderNotNull(builder);

            builder.Service.RegisterProvider(
                builder,
                new MemoryConfigurationProvider(
                    builder.CreateLogger(typeof(MemoryConfigurationProvider)),
                    builder.Service,
                    configurationEntries.AsEnumerable()));

            return builder;
        }

        public static IConfigurationBuilder AddEnvironmentVariables(this IConfigurationBuilder builder)
        {
            AssertBuilderNotNull(builder);

            builder.Service.RegisterProvider(
                builder,
                new EnvironmentVariablesConfigurationProvider(
                    builder.CreateLogger(typeof(EnvironmentVariablesConfigurationProvider)), builder.Service));

            return builder;
        }

        public static IConfigurationBuilder AddDotNetCoreConfiguration(this IConfigurationBuilder builder,
            IConfiguration configuration)
        {
            AssertBuilderNotNull(builder);

            builder.Service.RegisterProvider(
                builder,
                new DotNetCoreConfigurationProvider(builder.CreateLogger(typeof(DotNetCoreConfigurationProvider)),
                    builder.Service, configuration));

            return builder;
        }

        public static IConfigurationBuilder AddLogging(this IConfigurationBuilder builder, ILoggerFactory factory)
        {
            AssertBuilderNotNull(builder);

            builder.ConfigureLogging(factory.CreateLogger);

            return builder;
        }

        private static void AssertBuilderNotNull(IConfigurationBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
        }
    }
}