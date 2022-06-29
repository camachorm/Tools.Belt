using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.Storage.DNS.Extensions
{
    public static class TableResultExtensions
    {
        public static void EnsureNoErrors(this IList<TableResult> source)
        {
            IList<TableResult> failedResults = source.GetErrors();
            if (!failedResults.Any()) return;
            StorageException exception = new StorageException("Failed to perform batch save");
            foreach (TableResult failedResult in failedResults)
                exception.Data.Add($"{failedResult.Etag}({failedResult.HttpStatusCode})", failedResult.Result);
            throw exception;
        }

        public static void EnsureNoQueryErrors(this IList<TableResult> source)
        {
            source = source.Where(result => result.HttpStatusCode != 404).ToList();
            source.EnsureNoErrors();
        }

        public static void EnsureNoErrors(this TableResult source)
        {
            if (!source.HttpStatusCode.IsSuccessfulStatusCode())
            {
                StorageException exception = new StorageException("Table operation failed.");
                exception.Data.Add($"{source.Etag}({source.HttpStatusCode})", source.Result);
                throw exception;
            }
        }

        public static IList<TableResult> GetErrors(this IList<TableResult> source)
        {
            return source.Where(result => !result.HttpStatusCode.IsSuccessfulStatusCode()).ToList();
        }

        public static bool HasErrors(this IList<TableResult> source)
        {
            return source.GetErrors().Any();
        }

        public static List<List<T>> ChunkBy<T>(this IList<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new {Index = i, Value = x})
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}