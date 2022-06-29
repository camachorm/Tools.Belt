using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Tools.Belt.Azure.DNS.Extensions;
using Tools.Belt.Azure.Storage.DNS.Blob;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.Storage.DNS.Timer
{
    /// <summary>
    /// Helps keep track of a <see cref="TimerInfo"/> time period by using a cycle-based system,
    /// where a single cycle is equal to the period of the <see cref="TimerInfo"/>.
    /// To start a cycle call "Create" method, to end the cycle call the "Complete" method.
    /// Successful cycle execution time is saved on completion in the timer file, retrieved when a new cycle is created.
    /// </summary>
    public class TimerCycle
    {
        private readonly IBlobStorageService storage;

        private TimerCycle(
            DateTime start,
            DateTime end,
            TimeSpan period,
            string container,
            string timerFilename,
            IBlobStorageService storage)
        {
            this.CreationTime = DateTime.Now;
            this.StartTime = start;
            this.EndTime = end;
            this.Period = period;
            this.Container = container;
            this.TimerFilename = timerFilename;
            this.storage = storage;
        }

        /// <summary>
        /// The time at which the <see cref="TimerCycle"/> was created.
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// The cycle start time, retrieved either from the <see cref="TimerInfo"/> or from the timer file.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// The cycle end time, calculated based on the <see cref="TimerInfo"/> period.
        /// </summary>
        public DateTime EndTime { get; }
        
        /// <summary>
        /// The <see cref="TimerInfo"/> period.
        /// </summary>
        public TimeSpan Period { get; }

        /// <summary>
        /// The <see cref="TimeSpan"/> of time between the cycle start and end time.
        /// </summary>
        public TimeSpan Length => EndTime - StartTime;

        /// <summary>
        /// The time elapsed since the creation of the <see cref="TimerCycle"/> class.
        /// </summary>
        public TimeSpan TimeSinceCreation => DateTime.Now - CreationTime;

        /// <summary>
        /// The time elapsed since the start of the cycle.
        /// </summary>
        public TimeSpan TimeSinceStart => DateTime.Now - StartTime;

        /// <summary>
        /// The time remaining until the end of the cycle.
        /// </summary>
        public TimeSpan TimeUntilEnd => EndTime - DateTime.Now;

        /// <summary>
        /// Container used for the timer file.
        /// </summary>
        public string Container { get; }

        /// <summary>
        /// Filename used for the timer file.
        /// </summary>
        public string TimerFilename { get; }

        /// <summary>
        /// Maximum periods to catch up to if too many were missed.
        /// </summary>
        /// <example>
        /// A timer that runs every 1 minute would have a period of 1 minute.
        /// The last time the timer ran successfully was at 10:00.
        /// If that timer failed to execute 30 times, the time is now 10:30.
        /// The cycle now starts from 10:00 and ends at 10:30, causing a cycle length that is 30 times more than
        /// what is expected, essentially a cycle that contains 30 timer periods in it. 
        /// As such, this can cause system overload in certain cases.
        /// To prevent that, the amount of periods can be limited using this number.
        /// If the number is set to 5, the aforementioned timer would instead run with start time 10:00 and end time 10:05,
        /// gradually catching up.
        /// </example>
        public int MaxPeriodsWithinOneCycle = int.MaxValue;

        /// <summary>
        /// Creates a <see cref="TimerCycle"/> class.
        /// </summary>
        /// <param name="timer">Timer info on which the cycle is based.</param>
        /// <param name="container">Container to save the timer file.</param>
        /// <param name="timerFilename">Timer filename.</param>
        /// <param name="storage">Storage in which to save the timer file.</param>
        /// <param name="logger">Logger to log information.</param>
        /// <param name="maxPeriodsWithinOneCycle">Maximum periods in one cycle, preventing system overload on too many
        /// unsuccessful cycles.</param>
        public static async Task<TimerCycle> Create(
            TimerInfo timer,
            string container,
            string timerFilename,
            IBlobStorageService storage,
            ILogger logger,
            int maxPeriodsWithinOneCycle = 5)
        {
            TimeSpan period = timer.GetPeriod();
            DateTime endTime = timer.ScheduleStatus.Next;
            DateTime startTime = endTime - period;
            if (await storage.FileExistsAsync(container, timerFilename).ConfigureAwait(false))
            {
                using Stream stream = await storage.ReadFileAsStreamAsync(container, timerFilename).ConfigureAwait(false);
                string contents = await stream.ReadToEndAsync().ConfigureAwait(false);
                if (DateTime.TryParse(contents, out DateTime dateFromFile)) 
                    startTime = dateFromFile;
                else
                    logger.LogWarning($"Failed to parse timer date '{contents}' from file '{timerFilename}'. Starting new cycle.");
            }

            TimeSpan maxLength = TimeSpan.FromTicks(period.Ticks * maxPeriodsWithinOneCycle);
            if ((endTime - startTime) > maxLength) endTime = startTime + maxLength;

            var cycle = new TimerCycle(startTime, endTime, period, container, timerFilename, storage);
            return cycle;
        }

        /// <summary>
        /// Completes the cycle, saving the time of completion into the timer file.
        /// </summary>
        public async Task Complete()
        {
            string newContents = EndTime.ToString("O");
            await storage.WriteFileAsBlockBlobAsync(Container, TimerFilename, newContents).ConfigureAwait(false);
        }
    }
}
