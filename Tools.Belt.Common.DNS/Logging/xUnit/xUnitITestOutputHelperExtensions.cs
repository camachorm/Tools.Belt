using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Logging.xUnit
{
    // ReSharper disable InconsistentNaming
    public static class xUnitITestOutputHelperExtensions
        // ReSharper restore InconsistentNaming
    {
        public static ILoggerFactory GetLoggerFactory(this ITestOutputHelper testOutput)
        {
            LoggerFactory factory = new LoggerFactory();
#pragma warning disable CA2000 // Dispose objects before losing scope
            factory.AddProvider(new xUnitLoggerProvider(testOutput));
#pragma warning restore CA2000 // Dispose objects before losing scope
            return factory;
        }
    }
}