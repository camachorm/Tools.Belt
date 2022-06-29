using System;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Abstractions.Configuration
{
    public interface IConfigurationBuilder
    {
        IConfigurationBuilderOptionsModel Options { get; }

        IConfigurationService Service { get; }
        Func<Type, ILogger> CreateLogger { get; }

        IConfigurationService Build();

        void ConfigureLogging(Func<Type, ILogger> createLoggerFunction);
    }
}