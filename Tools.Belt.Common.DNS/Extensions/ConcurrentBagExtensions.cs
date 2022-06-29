using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tools.Belt.Common.Extensions
{
    public static class ConcurrentBagExtensions
    {
        [ExcludeFromCodeCoverage]
        public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> toAdd)
        {
            if (bag == null) throw new ArgumentNullException(nameof(bag));

            foreach (var element in toAdd)
            {
                bag.Add(element);
            }
        }
    }
}