using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools.Belt.Common.Exceptions;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Extensions
{
    public static class StringValuesExtensions
    {
        private delegate bool ParseFunction<T>(string a, out T b);

        /// <summary>
        /// Parses the values and returns the first one. Useful when the values parameter contains only a single value.
        /// If none, returns the default value.
        /// </summary>
        /// <param name="values">Values to parse.</param>
        /// <param name="ignoreCase">Case sensitivity.</param>
        /// <returns>Parsed enums.</returns>
        /// <exception cref="StringValuesConversionException"></exception>
        public static T ToEnum<T>(this StringValues values, bool ignoreCase = true)
            where T : struct => values.ToEnums<T>(ignoreCase).FirstOrDefault();
                
        /// <summary>
        /// Parses the values.
        /// </summary>
        /// <param name="values">Values to parse.</param>
        /// <param name="ignoreCase">Case sensitivity.</param>
        /// <returns>The first of the parsed enums.</returns>
        /// <exception cref="StringValuesConversionException"></exception>
        public static IEnumerable<T> ToEnums<T>(this StringValues values, bool ignoreCase = true)
            where T : struct
        {
            ParseFunction<T> func;
            if (ignoreCase) func = EnumTryParseIgnoreCase;
            else func = Enum.TryParse;

            return values.ToEnumerable(func);
        }
        
        /// <summary>
        /// Parses the values to UTC datetime and returns the first one. Useful when the values parameter contains only a single value.
        /// If none, returns the default value.
        /// </summary>
        /// <param name="values">Values to parse.</param>
        /// <returns>The first of the parsed dates.</returns>
        /// <exception cref="StringValuesConversionException"></exception>
        public static DateTime ToDateTimeUtc(this StringValues values)
            => values.ToDateTimesUtc().FirstOrDefault();

        /// <summary>
        /// Parses the values to UTC dates.
        /// </summary>
        /// <param name="values">Values to parse.</param>
        /// <returns>Parsed dates.</returns>
        /// <exception cref="StringValuesConversionException"></exception>
        public static IEnumerable<DateTime> ToDateTimesUtc(this StringValues values)
        {
            ParseFunction<DateTime> func = TryParseToUtc;
            return values.ToEnumerable(func);
        }

        private static bool TryParseToUtc(string a, out DateTime b)
        {
            return a.TryParseToUtc(out b);
        }

        private static IEnumerable<T> ToEnumerable<T>(this StringValues values, ParseFunction<T> func)
        {
            var results = new List<T>();

            foreach (var value in values)
            {
                if (func(values, out T result) == false)
                    throw new StringValuesConversionException($"Unable to parse value '{value}'.");
                results.Add(result);
            }

            return results;
        }

        private static bool EnumTryParseIgnoreCase<T>(string value, out T result)
             where T : struct
        {
            bool parse = Enum.TryParse(value, true, out T parseResult);
            result = parseResult;
            return parse;
        }
    }
}