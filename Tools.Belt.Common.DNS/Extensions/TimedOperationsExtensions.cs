using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Extensions
{
    public static class TimedOperationsExtensions
    {
        private static readonly ConcurrentDictionary<string, Stopwatch> Timers =
            new ConcurrentDictionary<string, Stopwatch>();

        /// <summary>
        ///     Extension method that times the execution of the provided synchronous <see cref="Action" />.
        /// </summary>
        /// <param name="action">A <see cref="Action" /> to execute.</param>
        /// <param name="logger">A instance of <see cref="ILogger" />.</param>
        /// <param name="taskIdentifier">The <see cref="string" /> that identifies the operation to be executed.</param>
        public static void TimeOperation(this Action action, ILogger logger, string taskIdentifier = "UnknownTask")
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!(logger?.IsEnabled(LogLevel.Trace)).GetValueOrDefault())
            {
                action.Invoke();
                return;
            }

            StartTimer(taskIdentifier, logger);
            action.Invoke();
            StopTimer(taskIdentifier, logger);
        }
        
        /// <summary>
        ///     Extension method that times the execution of the provided asynchronous <see cref="function" />.
        /// </summary>
        /// <param name="function">A <see cref="Func{TResult}" /> returning a <see cref="Task" /> object to be awaited on.</param>
        /// <param name="logger">A instance of <see cref="ILogger" />.</param>
        /// <param name="taskIdentifier">The <see cref="string" /> that identifies the operation to be executed.</param>
        /// <returns>A <see cref="Task" /> to be awaited on.</returns>
        public static async Task TimeAsyncOperation(this Func<Task> function, ILogger logger,
            string taskIdentifier = "UnknownTask")
        {
            if (function == null) throw new ArgumentNullException(nameof(function));
            if (!(logger?.IsEnabled(LogLevel.Trace)).GetValueOrDefault())
            {
                await function.Invoke().ConfigureAwait(false);
                return;
            }

            StartTimer(taskIdentifier, logger);
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await function.Invoke().ContinueWith(task => { 
                if (task.Exception != null) throw task.Exception;
                StopTimer(taskIdentifier, logger); 
            }, TaskScheduler.Current);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }

        
        public static async Task<T> TimeAsyncOperation<T>(this Task<T> function, ILogger logger,
            string taskIdentifier = "UnknownTask")
        {
            if (function == null) throw new ArgumentNullException(nameof(function));
            if (!(logger?.IsEnabled(LogLevel.Trace)).GetValueOrDefault())
            {
                return await function.ConfigureAwait(false);
            }

            StartTimer(taskIdentifier, logger);
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await function.ContinueWith(task => { 
                if (task.Exception != null) throw task.Exception;
                StopTimer(taskIdentifier, logger); 
            }, TaskScheduler.Current);
            
            return await function.ConfigureAwait(false);;
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }

        /// <summary>
        ///     Extension method that times the execution of the provided asynchronous <see cref="function" />.
        /// </summary>
        /// <param name="function">A <see cref="Func{TResult}" /> returning a <see cref="Task{T}" /> object to be awaited on.</param>
        /// <param name="logger">A instance of <see cref="ILogger" />.</param>
        /// <param name="taskIdentifier">The <see cref="string" /> that identifies the operation to be executed.</param>
        /// <typeparam name="T">The type of the result of the provided <see cref="function" /></typeparam>
        /// <returns>A <see cref="Task" /> to be awaited on.</returns>
        public static async Task<T> TimeAsyncOperation<T>(this Func<Task<T>> function, ILogger logger,
            string taskIdentifier = "UnknownTask")
        {
            if (function == null) throw new ArgumentNullException(nameof(function));
            if (!(logger?.IsEnabled(LogLevel.Trace)).GetValueOrDefault())
                return await function.Invoke().ConfigureAwait(false);
            StartTimer(taskIdentifier, logger);
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            T result = default;
            await function.Invoke().ContinueWith(task =>
            {
                StopTimer(taskIdentifier, logger);
                result = task.Result;
            }, TaskScheduler.Current);
            return result;
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        }

        /// <summary>
        ///     Registers a new running <see cref="Stopwatch" /> for an operation.
        ///     Note - This method is thread safe.
        /// </summary>
        /// <param name="operationName">The name of the operation to be timed.</param>
        /// <param name="logger">An <see cref="ILogger" /> </param>
        public static void StartTimer(string operationName, ILogger logger)
        {
            if (!(logger?.IsEnabled(LogLevel.Trace)).GetValueOrDefault())
                return;

            Timers.TryAdd(operationName, Stopwatch.StartNew());
        }

        /// <summary>
        ///     Stops a running timer and removes it from the in memory <see cref="Dictionary{TKey,TValue}" />
        ///     Note - This method is thread safe.
        /// </summary>
        /// <param name="operationName">The name of the operation to be timed.</param>
        /// <param name="logger">An <see cref="ILogger" /> </param>
        public static void StopTimer(string operationName, ILogger logger)
        {
            if (!(logger?.IsEnabled(LogLevel.Trace)).GetValueOrDefault())
                return;

            Timers.TryRemove(operationName, out Stopwatch sw);

            if (sw != null)
            {
                sw.Stop();
                logger.LogTrace($"{operationName} took {sw.Elapsed}.");
            }
        }
    }
}