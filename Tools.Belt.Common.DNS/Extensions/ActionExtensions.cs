using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Tools.Belt.Common.Extensions
{
    public static class ActionExtensions
    {
        public static void WrapTestOperation(
            this Action actionToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log)
        {
            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));
            Action<string, string> logOperationResultAction =
                (key, message) => log.Add(key, message);
            actionToInvoke.WrapTestOperation(keyToLog, logOperationResultAction, logOperationResultAction);
        }

        public static void WrapTestOperation(
            this Action actionToInvoke,
            string keyToLog,
            ILogger logger)
        {
            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));

            Action<string, string> logOperationSuccessResultAction = (key, message) => logger.LogDebug($"[{keyToLog}] - {message}");
            
            Action<string, string> logOperationErrorResultAction = (key, message) =>
            {
                logger.LogError($"[{keyToLog}] - {message}");
            };

            actionToInvoke.WrapTestOperation(keyToLog, logOperationSuccessResultAction, logOperationErrorResultAction);
        }

        public static void WrapTestOperation(
            this Action actionToInvoke,
            string keyToLog,
            Action<string, string> logOperationSuccessResult,
            Action<string, string> logOperationErrorResult)
        {
            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));
            try
            {
                actionToInvoke.Invoke();
                logOperationSuccessResult.Invoke(keyToLog, "Success");
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                logOperationErrorResult.Invoke(keyToLog, e.ToErrorJToken().ToString());
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static void WrapAndTimeTestOperation(
            this Action actionToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log)
        {
            Action<string, string> logOperationResultAction =
                (key, message) => log.Add(key, message);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            actionToInvoke.WrapTestOperation(keyToLog, logOperationResultAction, logOperationResultAction);
            stopwatch.Stop();
            string finalLog = $"{log[keyToLog]} - took a total of {stopwatch.ElapsedMilliseconds}ms.";
            log[keyToLog] = finalLog;
        }

        public static void WrapAndTimeTestOperation(
            this Action actionToInvoke,
            string keyToLog,
            Action<string> logOperation)
        {
            Action<string, string> logOperationResultAction =
                (key, message) => logOperation.Invoke(BuildLogEntry(key, message));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            actionToInvoke.WrapTestOperation(keyToLog, logOperationResultAction, logOperationResultAction);
            stopwatch.Stop();
            logOperation.Invoke(BuildLogEntry(keyToLog, stopwatch));
        }

        private static string BuildLogEntry(string keyToLog, string message)
        {
            return $"[{keyToLog}] - {message}";
        }

        public static void TimeOperation(
            this Action actionToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log)
        {
            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));
            if (log == null) throw new ArgumentNullException(nameof(log));
            actionToInvoke.TimeOperation(keyToLog, s => log[keyToLog] = new JValue(s));
        }

        public static void TimeOperation(
            this Action actionToInvoke,
            string keyToLog, ILogger logger)
        {
            actionToInvoke.TimeOperation(keyToLog, s => logger.LogDebug(s));
        }

        public static void TimeOperation(
            this Action actionToInvoke,
            string keyToLog, Action<string> logOperation)
        {
            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));
            if (logOperation == null) throw new ArgumentNullException(nameof(logOperation));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            actionToInvoke.Invoke();
            stopwatch.Stop();
            logOperation.Invoke(BuildLogEntry(keyToLog, stopwatch));
        }

        private static string BuildLogEntry(string keyToLog, Stopwatch stopwatch)
        {
            return $"{keyToLog} - took a total of {stopwatch.ElapsedMilliseconds}ms.";
        }
    }
}