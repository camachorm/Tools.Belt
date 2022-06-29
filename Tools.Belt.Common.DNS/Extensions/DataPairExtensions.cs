using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;

namespace Tools.Belt.Common.Extensions
{
    public static class DataPairExtensions
    {
        [ExcludeFromCodeCoverage]
        public static string ToPrintableString<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> source,
            bool censorValues = false)
        {
            return source?.Aggregate("",
                (current, pair) =>
                    current +
                    $"{pair.Key} / {(censorValues ? pair.Value?.ToString().ToPrintableSecret() : pair.Value?.ToString())}");
        }

        [ExcludeFromCodeCoverage]
        public static string ToPrintableString(this WebHeaderCollection source)
        {
            return source?.ToKeyValuePairs().ToPrintableString();
        }

        [ExcludeFromCodeCoverage]
        public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            NameValueCollection nameCollection = new NameValueCollection();

            foreach (KeyValuePair<string, string> tuple in source) nameCollection.Add(tuple.Key, tuple.Value);

            return nameCollection;
        }

        [ExcludeFromCodeCoverage]
        public static NameValueCollection ToNameValueCollection(this IEnumerable<Tuple<string, string>> source)
        {
            return source?.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2).ToNameValueCollection();
        }
    }
}