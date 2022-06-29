// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using System.Linq;

namespace Tools.Belt.Common.Extensions
{
    public static class StringEnumerableExtensions
    {
        /// <summary>
        ///     Get a single string from many strings, each separated by the <see cref="separatorString" /> parameter.
        /// </summary>
        /// <param name="source">The enumerable list of strings to flatten.</param>
        /// <param name="separatorString">
        ///     The separator character to use between strings. If this parameter is null, uses
        ///     <see cref="System.Environment.NewLine" />
        /// </param>
        /// <returns>The flattened string.</returns>
        public static string Flatten(this IEnumerable<string> source, string separatorString = null)
        {
            if (separatorString.IsNullOrEmpty()) separatorString = System.Environment.NewLine;

            return source?.Aggregate("", (current, newLine) => current + separatorString + newLine).Trim();
        }


        /// <summary>
        ///     Get a single comma separated string from many string values.
        /// </summary>
        /// <param name="source">The enumerable list of strings to flatten.</param>
        /// <returns>The flattened CSV string.</returns>
        public static string ToCsv(this IEnumerable<string> source)
        {
            return source?.Flatten(",");
        }
    }
}