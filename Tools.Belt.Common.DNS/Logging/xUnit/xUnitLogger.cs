// ReSharper disable LocalizableElement

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Mocks;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Logging.xUnit
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Global
    [ExcludeFromCodeCoverage]
    public class xUnitLogger : ILogger
        // ReSharper restore UnusedMember.Global
        // ReSharper restore InconsistentNaming
    {
        private readonly ITestOutputHelper _testOutput;

        public xUnitLogger(string categoryName, ITestOutputHelper testOutput)
        {
            CategoryName = categoryName;
            _testOutput = testOutput;
        }

        public string CategoryName { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            // This is required for the VS Output window
            _testOutput.WriteLine(formatter.Invoke(state, exception));
            // This is required for the build pipeline logging
            Console.WriteLine($"xUnit.Console: {formatter.Invoke(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new DisposableScope();
        }
    }
}