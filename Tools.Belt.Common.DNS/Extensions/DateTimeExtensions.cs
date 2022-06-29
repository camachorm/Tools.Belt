using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Tools.Belt.Common.Models;

namespace Tools.Belt.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        ///     Transform Base64 string in a readable string.
        /// </summary>
        /// <param name="base64Encoded">The encoded string.</param>
        /// <returns>The converted string or empty string if is not a Base64 string.</returns>
        public static DateTime FromBase64(string base64Encoded)
        {
            // Try to convert
            try
            {
                byte[] buffer = Convert.FromBase64String(base64Encoded);
                DateTime output = DateTime.Parse(Encoding.ASCII.GetString(buffer),CultureInfo.InvariantCulture);
                return output;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception err)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Console.WriteLine("Unable to Read from Base64 : " + err.Message);
            }

            // Is not a date encoded
            return DateTime.Now;
        }

        /// <summary>
        ///     Transform string into a Base64 encoded string.
        /// </summary>
        /// <param name="d">Current implementation of DateTime.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string ToBase64(this DateTime d)
        {
            d = d.ToUniversalTime();
            byte[] buffer = Encoding.ASCII.GetBytes(d.ToString("yyyy-MM-dd HH:mm:ss.ffffff+00:00", CultureInfo.InvariantCulture));
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        ///     Returns the DateTime at the start of the week.
        /// </summary>
        /// <param name="d">DateTime to process.</param>
        /// <returns>The DateTime marking the start of the week.</returns>
        public static DateTime StartOfWeek(this DateTime d, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (d.DayOfWeek - startOfWeek)) % 7;
            return d.AddDays(-1 * diff).Date;
        }

        /// <summary>
        ///     Returns the DateTime at the end of the week.
        /// </summary>
        /// <param name="d">DateTime to process.</param>
        /// <returns>The DateTime marking the end of the week.</returns>
        public static DateTime EndOfWeek(this DateTime d, DayOfWeek endOfWeek = DayOfWeek.Sunday)
        {
            int diff = (7 + (endOfWeek - d.DayOfWeek)) % 7;
            return d.AddDays(diff).Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        ///     Returns the DateTime at the start of the month.
        /// </summary>
        /// <param name="d">DateTime to process.</param>
        /// <returns>The DateTime marking the start of the month.</returns>
        public static DateTime StartOfMonth(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1, 0, 0, 0, d.Kind);
        }

        /// <summary>
        ///     Returns the DateTime at the end of the month.
        /// </summary>
        /// <param name="d">DateTime to process.</param>
        /// <returns>The DateTime marking the end of the month.</returns>
        public static DateTime EndOfMonth(this DateTime d)
        {
            return d.StartOfMonth().AddMonths(1).AddTicks(-1);
        }

        /// <summary>
        ///     Returns the DateTime at the start of the year.
        /// </summary>
        /// <param name="d">DateTime to process.</param>
        /// <returns>The DateTime marking the start of the year.</returns>
        public static DateTime StartOfYear(this DateTime d)
        {
            return new DateTime(d.Year, 1, 1, 0, 0, 0, d.Kind);
        }

        /// <summary>
        ///     Returns the DateTime at the end of the year.
        /// </summary>
        /// <param name="d">DateTime to process.</param>
        /// <returns>The DateTime marking the end of the year.</returns>
        public static DateTime EndOfYear(this DateTime d)
        {
            return d.StartOfYear().AddYears(1).AddTicks(-1);
        }

        /// <summary>
        /// Get all days between dates, inclusive.
        /// </summary>
        /// <param name="from">Date to start from.</param>
        /// <param name="to">Date to end at.</param>
        /// <returns>A list of all days between the dates.</returns>
        public static IEnumerable<DateTime> GetAllDaysUntil(this DateTime from, DateTime to, 
            RangeBehaviour rBehaviour = RangeBehaviour.Inclusive)
        {
            from = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, from.Kind);
            if (rBehaviour == RangeBehaviour.Exclusive || rBehaviour == RangeBehaviour.FromExclusiveToInclusive)
                from = from.AddDays(1);
            to = new DateTime(to.Year, to.Month, to.Day, 0, 0, 0, from.Kind);
            if (rBehaviour == RangeBehaviour.Exclusive || rBehaviour == RangeBehaviour.FromInclusiveToExclusive)
                to = from.AddDays(-1);

            var days = new List<DateTime>();
            while (from <= to)
            {
                days.Add(from);
                from = from.AddDays(1);
            }

            return days;
        }
    }
}