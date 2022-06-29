using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Abstractions.Configuration.Extensions;
using Tools.Belt.Common.Logging.xUnit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Tests.xUnit
{
    // ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
    public abstract class xUnitTestBase<T> where T : xUnitTestBase<T>
#pragma warning restore IDE1006 // Naming Styles
    // ReSharper restore InconsistentNaming
    {
        // ReSharper disable StaticMemberInGenericType
        private static readonly IConfigurationRoot CoreConfig = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", true, true)
            .AddEnvironmentVariables()
            .Build();
        // ReSharper restore StaticMemberInGenericType

        protected xUnitTestBase(ITestOutputHelper testOutputHelper)
        {
            LoggerFactory = testOutputHelper.GetLoggerFactory();
            Logger = LoggerFactory.CreateLogger<T>();

            Configuration = _builder.Build();
        }

        private readonly Abstractions.Configuration.IConfigurationBuilder _builder = new Abstractions.Configuration.Builder.ConfigurationBuilder()
            .AddDotNetCoreConfiguration(CoreConfig);

        protected ILoggerFactory LoggerFactory { get; set; }
        protected ILogger<T> Logger { get; set; }
        protected IConfigurationService Configuration { get; set; }

        protected void SetupInMemoryVariableS(ConcurrentDictionary<string, string> configurationEntries)
        {
            SetupInMemoryVariableS(configurationEntries.AsEnumerable());
        }
        
        protected void SetupInMemoryVariableS(Dictionary<string, string> configurationEntries)
        {
            SetupInMemoryVariableS(configurationEntries.AsEnumerable());
        }
        
        protected void SetupInMemoryVariableS(IEnumerable<KeyValuePair<string, string>> configurationEntries)
        {
            Configuration = _builder.AddInMemoryConfiguration(configurationEntries).Build();
        }
    }
}