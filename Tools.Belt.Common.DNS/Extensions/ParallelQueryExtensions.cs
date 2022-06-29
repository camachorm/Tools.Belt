using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Belt.Common.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class ParallelQueryExtensions
    {
        /// <summary>
        /// Invokes in parallel the specified function for each element in the source.
        /// </summary>
        public static async Task ForAllAsync<TSource>(
            this ParallelQuery<TSource> source, 
            Func<TSource, Task> selector,
            int? maxDegreeOfParallelism = null)
        {
            int maxAsyncThreadCount = maxDegreeOfParallelism ?? Math.Min(System.Environment.ProcessorCount, 128);
            using SemaphoreSlim throttler = new SemaphoreSlim(maxAsyncThreadCount, maxAsyncThreadCount);

            IEnumerable<Task> tasks = source.Select(async input =>
            {
                // ReSharper disable AccessToDisposedClosure
                await throttler.WaitAsync().ConfigureAwait(false);
                
                try
                {
                    await selector(input).ConfigureAwait(false);
                }
                finally
                {
                    throttler.Release();
                    // ReSharper restore AccessToDisposedClosure
                }
            });

            await Task.WhenAll(tasks).ConfigureAwait(true);
        }
        
        /// <summary>
        /// Invokes in parallel the specified function for each element in the source.
        /// </summary>
        public static async Task<IEnumerable<T>> ForAllAsync<TSource, T>(
            this ParallelQuery<TSource> source,
            Func<TSource, Task<T>> selector,
            int? maxDegreeOfParallelism = null) 
            where T : new()
        {
            int maxAsyncThreadCount = maxDegreeOfParallelism ?? Math.Min(System.Environment.ProcessorCount, 128);
            using SemaphoreSlim throttler = new SemaphoreSlim(maxAsyncThreadCount, maxAsyncThreadCount);

            IEnumerable<Task<T>> tasks = source.Select(async input =>
            {
                T result = new T();

                await throttler.WaitAsync().ConfigureAwait(false);
                try
                {
                    result = await selector(input).ConfigureAwait(false);
                }
                finally
                {
                    throttler.Release();
                }

                return result;
            });

            return await Task.WhenAll(tasks).ConfigureAwait(true);
        }
    }
}
