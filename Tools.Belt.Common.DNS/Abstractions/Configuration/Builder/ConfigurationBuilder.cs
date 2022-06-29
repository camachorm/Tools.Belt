using System;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration.Service;
using Tools.Belt.Common.Mocks;

namespace Tools.Belt.Common.Abstractions.Configuration.Builder
{
    public sealed class ConfigurationBuilder : IConfigurationBuilder
    {
        public ConfigurationBuilder()
        {
            Service = new ConfigurationService(this);
        }

        public IConfigurationBuilderOptionsModel Options { get; } = new ConfigurationBuilderOptionsModel();

        public IConfigurationService Service { get; }

        public IConfigurationService Build()
        {
            return Service;
        }

        public void ConfigureLogging(Func<Type, ILogger> createLoggerFunction)
        {
            CreateLogger = createLoggerFunction;
        }

        public Func<Type, ILogger> CreateLogger { get; private set; } = type => new DebugLogger();
    }
}