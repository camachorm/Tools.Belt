using System;
using System.Globalization;

namespace Tools.Belt.Common.Extensions
{
    public static class StringDateTimeExtensions
    {
        public static bool IsValidDateTime(
            this string source,
            string cultureInfoSettingsKey = "DateTimeCultureInfo",
            DateTimeStyles style = DateTimeStyles.None)
        {
            return source.ToDateTime(cultureInfoSettingsKey, style) != null;
        }

        /// <summary>
        /// Parses a datetime string to UTC.
        /// </summary>
        /// <param name="dateTimeStr">Datetime string to parse.</param>
        /// <returns>Parsed Datetime.</returns>
        public static DateTime ParseToUtc(this string dateTimeStr)
        {
            return DateTime.Parse(dateTimeStr).ToUniversalTime();
        }

        /// <summary>
        /// Tries to parse a datetime string to UTC.
        /// </summary>
        /// <param name="dateTimeStr">Datetime string to parse.</param>
        /// <param name="result">Parsed datetime.</param>
        /// <returns>A boolean representing success/fail of the parsing.</returns>
        public static bool TryParseToUtc(this string dateTimeStr, out DateTime result)
        {
            var parsed = DateTime.TryParse(dateTimeStr, out result);

            if (parsed)
                result = result.ToUniversalTime();
            
            return parsed;
        }
    }
}