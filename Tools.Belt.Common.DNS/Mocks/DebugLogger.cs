using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Mocks
{
    /// <summary>
    ///     This class writes to Console, DebugLogger and Trace
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Obsolete]
    public class DebugLogger : ILogger
    {
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            // ReSharper disable LocalizableElement
            string message =
                $"Level: {logLevel}, eventId: {eventId}, state: {state}, exception: {exception?.Message}, formatter: {formatter.Invoke(state, exception)}";
            // ReSharper restore LocalizableElement
            Console.WriteLine(message);

            Debug.WriteLine(message);
            Trace.TraceInformation(message);
            Logs.Add(CleanMessage(formatter,state,exception));
        }

        private string CleanMessage<TState>(Func<TState, Exception, string> formatter, TState state, Exception exception)
        {
            var message = formatter.Invoke(state, exception);
            if (message.StartsWith(": ",StringComparison.InvariantCultureIgnoreCase))
            {
                message = message.Substring(2);
            }
            return message;
        }

        public IList<string> Logs { get; set; } = new List<string>();

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