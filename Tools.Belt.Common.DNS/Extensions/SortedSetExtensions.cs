using System.Collections.Generic;

namespace Tools.Belt.Common.Extensions
{
    public static class SortedSetExtensions
    {
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source)
        {
            return new SortedSet<T>(source);
        }
    }
}