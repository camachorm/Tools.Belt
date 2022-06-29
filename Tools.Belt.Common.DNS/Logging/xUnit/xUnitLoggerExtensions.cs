using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Logging.xUnit
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Global
    public static class xUnitLoggerExtensions
        // ReSharper restore UnusedMember.Global
        // ReSharper restore InconsistentNaming
    {
        private static xUnitLoggerProvider _provider;

        public static ILoggerFactory AddXUnitLogger(this ILoggerFactory source, ITestOutputHelper testOutput)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            _provider = new xUnitLoggerProvider(testOutput);
            source.AddProvider(_provider);
            return source;
        }
    }
}