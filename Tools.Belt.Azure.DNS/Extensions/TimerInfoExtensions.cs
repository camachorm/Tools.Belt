using Microsoft.Azure.WebJobs;
using System;

namespace Tools.Belt.Azure.DNS.Extensions
{
    public static class TimerInfoExtensions
    {
        /// <summary>
        /// Gets the time period of the timer.
        /// </summary>
        /// <param name="source">Timer to process.</param>
        public static TimeSpan GetPeriod(this TimerInfo source)
        {
            var timerPeriod = source.Schedule.GetNextOccurrence(source.ScheduleStatus.Next) - source.ScheduleStatus.Next;
            return timerPeriod;
        }
    }
}