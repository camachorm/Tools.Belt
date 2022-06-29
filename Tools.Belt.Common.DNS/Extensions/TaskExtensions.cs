using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Belt.Common.Extensions
{
    public static class TaskExtensions
    {
        public static void RunTaskSynchronously(this Task source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.Wait();
        }

        public static TOut RunSynchronously<TOut>(this Task<TOut> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.Wait();
            return source.Result;
        }

        public static TOut RunSynchronously<TOut>(this Task<TOut> source, CancellationToken cancellationToken)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.Wait(cancellationToken);
            return source.Result;
        }

        public static TOut RunSynchronously<TOut>(this Task<TOut> source, int millisecondsToTimeOut)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.Wait(millisecondsToTimeOut);
            return source.Result;
        }

        public static TOut RunSynchronously<TOut>(this Task<TOut> source, int millisecondsToTimeOut,
            CancellationToken cancellationToken)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.Wait(millisecondsToTimeOut, cancellationToken);
            return source.Result;
        }

        public static TOut RunSynchronously<TOut>(this Task<TOut> source, TimeSpan timeSpanToTimeOut)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.Wait(timeSpanToTimeOut);
            return source.Result;
        }

        public static TOut ReturnAsyncCallSynchronously<TOut>(this Task<TOut> taskToExecute)
        {
            return taskToExecute.ReturnAsyncCallSynchronously(CancellationToken.None);
        }

        [ExcludeFromCodeCoverage]
        public static TOut ReturnAsyncCallSynchronously<TOut>(this Task<TOut> taskToExecute,
            CancellationToken cancellationToken)
        {
            return taskToExecute.ReturnAsyncCallSynchronously(Int32.MaxValue, cancellationToken);
        }

        [ExcludeFromCodeCoverage]
        public static TOut ReturnAsyncCallSynchronously<TOut>(this Task<TOut> taskToExecute, int millisecondsToTimeOut)
        {
            return taskToExecute.ReturnAsyncCallSynchronously(millisecondsToTimeOut, CancellationToken.None);
        }

        [ExcludeFromCodeCoverage]
        public static TOut ReturnAsyncCallSynchronously<TOut>(this Task<TOut> taskToExecute, TimeSpan timeSpanToTimeOut)
        {
            return taskToExecute.ReturnAsyncCallSynchronously((int)timeSpanToTimeOut.TotalMilliseconds);
        }
        
        public static TOut ReturnAsyncCallSynchronously<TOut>(this Task<TOut> taskToExecute, int millisecondsToTimeOut,
            CancellationToken cancellationToken)
        {
            if (taskToExecute == null) throw new ArgumentNullException(nameof(taskToExecute));
            return !taskToExecute.Wait(millisecondsToTimeOut, cancellationToken)
                ? throw new TimeoutException($"Timeout of {millisecondsToTimeOut}ms. exceeded")
                : taskToExecute.Result;
        }
    }
}