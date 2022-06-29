using System;
using System.Collections.Generic;

namespace Tools.Belt.Common.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Compares the contents of two dictionaries and returns true if they are the same.
        /// </summary>
        public static bool CompareContents<TKey, TValue>(this IDictionary<TKey, TValue> a, IDictionary<TKey, TValue> b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            bool equal = a.Count == b.Count;
            if (equal == false) return equal;

            foreach (var prop in b)
            {
                if (a.ContainsKey(prop.Key) == false)
                {
                    return false;
                }
            }

            foreach (var prop in b)
            {
                if (a[prop.Key].Equals(b[prop.Key]) == false)
                {
                    return false;
                }
            }

            return equal;
        }
    }
}