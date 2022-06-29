using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tools.Belt.Common.Extensions
{
    public static class HttpHeadersExtensions
    {
        [ExcludeFromCodeCoverage]
        public static IEnumerable<KeyValuePair<string, string>> AddContentType(
            this IEnumerable<KeyValuePair<string, string>> source,
            string value)
        {
            List<KeyValuePair<string, string>> list = source.ToList();
            list.Add(new KeyValuePair<string, string>("Content-Type", value));
            return list;
        }
    }
}