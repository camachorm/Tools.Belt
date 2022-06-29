using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Tools.Belt.Common.Extensions
{
    public static class WebHeaderCollectionExtensions
    {
        [ExcludeFromCodeCoverage]
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(this WebHeaderCollection source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            foreach (string s in source.Keys) list.Add(new KeyValuePair<string, string>(s, source[s]));

            return list;
        }
    }
}