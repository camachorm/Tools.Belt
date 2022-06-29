using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Common.Wrappers
{
    /// <summary>
    ///     Create an instance of this class around a Task of your choice to have it timed until conclusion.
    ///     Particularly useful in some scenarios where you want to time individual tasks inline with your regular code
    ///     and you want to easily remove it later.
    /// </summary>
    public class TimedTaskWrapper
    {
        private readonly ILogger _logger;

        public TimedTaskWrapper(Task task, string identifier, ILogger logger)
        {
            StartedAt = DateTime.UtcNow;
            _logger = logger;
            Task = task;
            Identifier = identifier;
            _ = UpdateWhenFinishedAsync();
        }

        public Task Task { get; }
        public string Identifier { get; }

        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }

        public TimeSpan CompletedIn => CompletedAt.Subtract(StartedAt);

        private async Task UpdateWhenFinishedAsync()
        {
            _logger.LogTrace($"Started: {ToString()}");
            await Task.ConfigureAwait(false);
            CompletedAt = DateTime.UtcNow;
            _logger.LogTrace(ToString());
        }

        public override string ToString()
        {
            string appendix =
                $"completed: {Task.IsCompleted} at {CompletedAt.ToLongDateString()} and took {CompletedIn.TotalMilliseconds} ms.";
            return $"{Identifier}{(Task.IsCompleted ? appendix : string.Empty)}";
        }
    }
}