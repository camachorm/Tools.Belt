using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace Tools.Belt.Common.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions
    {

        public static Func<TMessageModel, Task>[] ToFilteredFunctionArray<TRequestModel, TMessageModel>(this IEnumerable<Tuple<Func<TRequestModel, bool>, Func<TMessageModel, Task>>> source, TRequestModel model)
        {
            return source.Where(tuple => tuple.Item1.Invoke(model)).Select(tuple => tuple.Item2)
                .ToArray();
        }

        public static async Task<string> ToCsvAsync<T>(this IEnumerable<T> source, bool writeHeaders = false) 
        {
            var ms = new MemoryStream();
            
            using var writer = new StreamWriter(ms);
           
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (writeHeaders)
                {
                    csvWriter.WriteHeader<T>();
                    await csvWriter.NextRecordAsync().ConfigureAwait(false);
                }

                foreach (var convertible in source)
                {
                    csvWriter.WriteRecord(convertible);
                    await csvWriter.NextRecordAsync().ConfigureAwait(false);
                }
            }

            return ms.ToArray().Decode();
        }
        /// <summary>
        /// Processes an enumerable asynchronously and launching multiple tasks in parallel for each entry in the enumerable, before awaiting on Task.WhenAll
        /// </summary>
        /// <typeparam name="TEnumerable">The type of the enumerable</typeparam>
        /// <typeparam name="TResult">The type to which each enumerable entry is to be converted</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="convertEnumerableToResultFunc">The func to convert from the enumerable type to the result type</param>
        /// <param name="processResultAsyncFuncs">An optional list of functions to be performed on the converted result</param>
        /// <returns></returns>
        public static async Task ProcessEnumerableAsync<TEnumerable, TResult>(this IEnumerable<TEnumerable> source,
            Func<TEnumerable, TResult> convertEnumerableToResultFunc,
            params Func<TResult, Task>[] processResultAsyncFuncs)
        {
            var taskList = new List<Task>();
            foreach (var enumerable in source)
            {
                var result = convertEnumerableToResultFunc(enumerable);
                if (processResultAsyncFuncs.Any())
                {
                    taskList.AddRange(processResultAsyncFuncs.Select(func => func.Invoke(result)));
                }
            }

            if (taskList.Any())
            {
                await Task.WhenAll(taskList);
            }
        }

        public static IEnumerable<IList<T>> Split<T>(this IEnumerable<T> source, int maximumGroupSize = 3)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerator<T> enumerator = source.GetEnumerator();
            List<T> batch = new List<T>();
            while (enumerator.MoveNext())
            {
                if (batch.Count >= maximumGroupSize)
                {
                    yield return batch;
                    batch = new List<T>();
                }

                batch.Add(enumerator.Current);
            }

            yield return batch;
            enumerator.Dispose();
        }

        /// <summary>
        /// Break a list of items into chunks of a specific size.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static IList<T> DistinctByWithPreservedOrder<T, TKey>(this IEnumerable<T> source, Func<T, TKey> comparer)
        {
            var result = new Dictionary<TKey, T>();
            foreach (var item in source.Where(item => !result.ContainsKey(comparer.Invoke(item))))
            {
                result.Add(comparer.Invoke(item), item);
            }

            return result.Values.ToList();
        }
    }
}