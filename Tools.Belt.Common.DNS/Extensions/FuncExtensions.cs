using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Tools.Belt.Common.Extensions
{
    public static class FuncExtensions
    {
        public static async Task WrapTestOperation(
            this Func<Task> actionToInvoke,
            string keyToLog,
            IDictionary<string, JToken> log,
            ILogger logger)
        {
            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));
            if (keyToLog.IsNullOrEmpty()) throw new ArgumentNullException(nameof(keyToLog));
            if (log == null) throw new ArgumentNullException(nameof(log));

            try
            {
                await actionToInvoke.Invoke().ConfigureAwait(false);
                log.Add(keyToLog, "Success");
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to perform operation against {keyToLog}: {e.FullStackTrace()}");
                log.Add(keyToLog, e.ToErrorJToken());
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static async Task<T> WrapTestOperation<T>(
            this Func<Task<T>> actionToInvoke,
            string keyToLog,
            IDictionary<string, JToken> log,
            ILogger logger)
        {
            T r = default;

            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));
            if (keyToLog.IsNullOrEmpty()) throw new ArgumentNullException(nameof(keyToLog));
            if (log == null) throw new ArgumentNullException(nameof(log));

            try
            {
                r = await actionToInvoke.Invoke().ConfigureAwait(false);
                log.Add(
                    keyToLog,
                    new JObject().AddProperties(
                        new KeyValuePair<string, JToken>("Status", "Success"),
                        new KeyValuePair<string, JToken>("Result", r.ToSafeJToken(logger))));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to perform operation against {keyToLog}: {e.FullStackTrace()}");
                log.Add(keyToLog, e.ToErrorJToken());
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return r;
        }

        public static T WrapTestOperation<T>(
            this Func<T> actionToInvoke,
            string keyToLog,
            IDictionary<string, JToken> log,
            ILogger logger)
        {
            T r = default;

            if (actionToInvoke == null) throw new ArgumentNullException(nameof(actionToInvoke));
            if (keyToLog.IsNullOrEmpty()) throw new ArgumentNullException(nameof(keyToLog));
            if (log == null) throw new ArgumentNullException(nameof(log));

            try
            {
                r = actionToInvoke.Invoke();
                log.Add(
                    keyToLog,
                    new JObject().AddProperties(
                        new KeyValuePair<string, JToken>("Status", "Success"),
                        new KeyValuePair<string, JToken>("Result", r.ToSafeJToken(logger))));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to perform operation against {keyToLog}: {e.FullStackTrace()}");
                log.Add(keyToLog, e.ToErrorJToken());
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return r;
        }

        public static void WrapAndTimeTestOperation(this Func<Task> asyncFuncToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log,
            ILogger logger)
        {
            Task t = asyncFuncToInvoke.WrapAndTimeTestOperationAsync(keyToLog, log, logger);
            t.Wait();
        }

        public static async Task WrapAndTimeTestOperationAsync(this Func<Task> asyncFuncToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log,
            ILogger logger)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await asyncFuncToInvoke.WrapTestOperation(keyToLog, log, logger).ConfigureAwait(false);
            stopwatch.Stop();
            string finalLog = $"{log[keyToLog]} - took a total of {stopwatch.ElapsedMilliseconds}ms.";
            log[keyToLog] = finalLog;
        }

        public static void TimeOperation(this Func<Task> asyncFuncToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log,
            ILogger logger)
        {
            Task t = asyncFuncToInvoke.TimeOperationAsync(keyToLog, log, logger);
            t.Wait();
        }

        public static async Task TimeOperationAsync(this Func<Task> asyncFuncToInvoke,
            string keyToLog,
            IDictionary<string, JToken> log,
            ILogger logger)
        {
            asyncFuncToInvoke = asyncFuncToInvoke ?? throw new ArgumentNullException(nameof(asyncFuncToInvoke));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await asyncFuncToInvoke.Invoke().ConfigureAwait(false);
            stopwatch.Stop();

            if (!log.ContainsKey(keyToLog))
            {
                string finalLog = $"{keyToLog} - took a total of {stopwatch.ElapsedMilliseconds}ms.";
                log.Add(keyToLog, finalLog);
            }

            else
            {
                string finalLog = $"{log[keyToLog]} - took a total of {stopwatch.ElapsedMilliseconds}ms.";
                log[keyToLog] = finalLog;
            }

        }


        public static async Task TimeOperationAsync(
            this Func<Task> asyncFuncToInvoke,
            string keyToLog,
            ILogger logger)
        {
            asyncFuncToInvoke = asyncFuncToInvoke ?? throw new ArgumentNullException(nameof(asyncFuncToInvoke));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await asyncFuncToInvoke.Invoke().ConfigureAwait(false);
            }
            finally
            {
                stopwatch.Stop();
                logger.LogTrace($"{keyToLog} - took a total of {stopwatch.ElapsedMilliseconds}ms.");
            }
        }

        public static async Task<T> TimeOperationAsync<T>(
            this Func<Task<T>> asyncFuncToInvoke,
            string keyToLog,
            ILogger logger)
        {
            asyncFuncToInvoke = asyncFuncToInvoke ?? throw new ArgumentNullException(nameof(asyncFuncToInvoke));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                return await asyncFuncToInvoke.Invoke().ConfigureAwait(false);
            }
            finally
            {
                stopwatch.Stop();
                logger.LogTrace($"{keyToLog} - took a total of {stopwatch.ElapsedMilliseconds}ms.");
            }
        }

        public static T TimeOperation<T>(this Func<Task<T>> asyncFuncToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log,
            ILogger logger)
        {
            Task<T> t = asyncFuncToInvoke.TimeOperationAsync(keyToLog, log, logger);
            t.Wait();
            return t.Result;
        }

        public static async Task<T> TimeOperationAsync<T>(this Func<Task<T>> asyncFuncToInvoke,
            string keyToLog,
            Dictionary<string, JToken> log,
            ILogger logger)
        {
            asyncFuncToInvoke = asyncFuncToInvoke ?? throw new ArgumentNullException(nameof(asyncFuncToInvoke));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                return await asyncFuncToInvoke.Invoke().ConfigureAwait(false);
            }
            finally
            {
                stopwatch.Stop();
                string finalLog = "";
                if (!log.ContainsKey(keyToLog))
                {
                    finalLog = $"{keyToLog} - took a total of {stopwatch.ElapsedMilliseconds}ms.";
                    log.Add(keyToLog, finalLog);

                }
                else
                {
                    finalLog = $"{log[keyToLog]} - took a total of {stopwatch.ElapsedMilliseconds}ms.";
                    log[keyToLog] = finalLog;

                }
                logger.LogDebug(finalLog);
            }
        }
    }
}