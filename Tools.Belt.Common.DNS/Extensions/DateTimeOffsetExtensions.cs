using System;
using System.Collections.Generic;
using Tools.Belt.Common.Models;

namespace Tools.Belt.Common.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        ///     Returns the DateTimeOffset at the start of the week.
        /// </summary>
        /// <param name="d">DateTimeOffset to process.</param>
        /// <returns>The DateTimeOffset marking the start of the week.</returns>
        public static DateTimeOffset StartOfWeek(this DateTimeOffset d, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (d.DayOfWeek - startOfWeek)) % 7;
            return d.AddDays(-1 * diff).Date;
        }

        /// <summary>
        ///     Returns the DateTimeOffset at the end of the week.
        /// </summary>
        /// <param name="d">DateTimeOffset to process.</param>
        /// <returns>The DateTimeOffset marking the end of the week.</returns>
        public static DateTimeOffset EndOfWeek(this DateTimeOffset d, DayOfWeek endOfWeek = DayOfWeek.Sunday)
        {
            int diff = (7 + (endOfWeek - d.DayOfWeek)) % 7;
            return d.AddDays(diff).Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        ///     Returns the DateTimeOffset at the start of the month.
        /// </summary>
        /// <param name="d">DateTimeOffset to process.</param>
        /// <returns>The DateTimeOffset marking the start of the month.</returns>
        public static DateTimeOffset StartOfMonth(this DateTimeOffset d)
        {
            return new DateTimeOffset(d.Year, d.Month, 1, 0, 0, 0, d.Offset);
        }

        /// <summary>
        ///     Returns the DateTimeOffset at the end of the month.
        /// </summary>
        /// <param name="d">DateTimeOffset to process.</param>
        /// <returns>The DateTimeOffset marking the end of the month.</returns>
        public static DateTimeOffset EndOfMonth(this DateTimeOffset d)
        {
            return d.StartOfMonth().AddMonths(1).AddTicks(-1);
        }

        /// <summary>
        ///     Returns the DateTimeOffset at the start of the year.
        /// </summary>
        /// <param name="d">DateTimeOffset to process.</param>
        /// <returns>The DateTimeOffset marking the start of the year.</returns>
        public static DateTimeOffset StartOfYear(this DateTimeOffset d)
        {
            return new DateTimeOffset(d.Year, 1, 1, 0, 0, 0, d.Offset);
        }

        /// <summary>
        ///     Returns the DateTimeOffset at the end of the year.
        /// </summary>
        /// <param name="d">DateTimeOffset to process.</param>
        /// <returns>The DateTimeOffset marking the end of the year.</returns>
        public static DateTimeOffset EndOfYear(this DateTimeOffset d)
        {
            return d.StartOfYear().AddYears(1).AddTicks(-1);
        }

        /// <summary>
        /// Get all days between dates, inclusive.
        /// </summary>
        /// <param name="from">Date to start from.</param>
        /// <param name="to">Date to end at.</param>
        /// <returns>A list of all days between the dates.</returns>
        public static IEnumerable<DateTimeOffset> GetAllDaysUntil(this DateTimeOffset from, DateTimeOffset to, 
            RangeBehaviour rBehaviour = RangeBehaviour.Inclusive)
        {
            from = new DateTimeOffset(from.Year, from.Month, from.Day, 0, 0, 0, from.Offset);
            if (rBehaviour == RangeBehaviour.Exclusive || rBehaviour == RangeBehaviour.FromExclusiveToInclusive)
                from = from.AddDays(1);
            to = new DateTimeOffset(to.Year, to.Month, to.Day, 0, 0, 0, to.Offset);
            if (rBehaviour == RangeBehaviour.Exclusive || rBehaviour == RangeBehaviour.FromInclusiveToExclusive)
                to = from.AddDays(-1);

            var days = new List<DateTimeOffset>();
            while (from.Date <= to.Date)
            {
                days.Add(from);
                from = from.AddDays(1);
            }

            return days;
        }

        /// <summary>
        /// Compares two nullable DateTimeOffset values.
        /// </summary>
        /// <param name="dt1">First date to compare.</param>
        /// <param name="dt2">Second date to compare</param>
        /// <returns></returns>
        public static int CompareTo(this DateTimeOffset? dt1, DateTimeOffset? dt2)
        {
            if (dt1 == null && dt2 == null) return 0;

            if (dt1.HasValue && !dt2.HasValue) return 1;

            if (!dt1.HasValue && dt2.HasValue) return -1;

            return dt1.Value.CompareTo(dt2.Value);
        }
    }
}