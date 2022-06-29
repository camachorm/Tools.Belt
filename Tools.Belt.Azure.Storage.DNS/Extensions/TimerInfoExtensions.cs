using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Tools.Belt.Azure.Storage.DNS.Blob;
using Tools.Belt.Azure.Storage.DNS.Timer;

namespace Tools.Belt.Azure.Storage.DNS.Extensions
{
    public static class TimerInfoExtensions
    {
        /// <summary>
        /// Creates a timer cycle from the given timer. Cycles are used to process time-sensitive data in periods of time.
        /// </summary>
        public static async Task<TimerCycle> CreateCycle(
            this TimerInfo source,
            string container,
            string timerFilename,
            IBlobStorageService storage,
            ILogger logger,
            int maxPeriodsWithinOneCycle = 5)
        {
            return await TimerCycle.Create(source, container, timerFilename, storage, logger, maxPeriodsWithinOneCycle).ConfigureAwait(false);
        }
    }
}