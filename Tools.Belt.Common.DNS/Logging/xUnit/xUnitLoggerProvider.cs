using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Logging.xUnit
{
    // ReSharper disable UnusedMember.Global
    // ReSharper disable InconsistentNaming
    public class xUnitLoggerProvider : ILoggerProvider
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Global
    {
        private readonly ITestOutputHelper _testOutput;

        public xUnitLoggerProvider(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new xUnitLogger(categoryName, _testOutput);
        }

        protected virtual void Dispose(bool dispose)
        {
        }
    }
}