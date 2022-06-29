using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    /// <remarks>
    ///     If the CPU has too many processes consuming time, this may cause it
    ///     to fail to spin enough threads, causing some of the tests to fail.
    ///     If that does happen, free some processing resources and try running the tests again.
    ///     This is an edge case that I could not reproduce even under artificial load, but still not impossible in theory.
    /// </remarks>
    public class ParallelQueryExtensionsTests
    {
        private const int delay = 10;

        [Fact]
        public void ForAllAsync_AsyncVoid_ThreadCountCorrect()
        {
            const int parallelism = 3;
            var processes = new object[30];

            var running = 0;
            var maxThreads = 0;

            var task = processes.AsParallel().ForAllAsync(async item => await Task.Run(async () =>
            {
                await Task.Delay(delay);
                Interlocked.Increment(ref running);
                await Task.Delay(delay);
                Interlocked.Decrement(ref running);
                await Task.Delay(delay);
            }), parallelism);
            while (task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingForActivation)
                maxThreads = maxThreads > running ? maxThreads : running;

            Assert.Equal(parallelism, maxThreads);
        }

        [Fact]
        public void ForAllAsync_AsyncVoid_FinishAllOnWait()
        {
            const int parallelism = 3;
            var processes = new object[10];

            var finished = 0;

            processes.AsParallel().ForAllAsync(async item => await Task.Run(async () =>
            {
                await Task.Delay(delay);
                Interlocked.Increment(ref finished);
            }), parallelism).Wait();

            Assert.Equal(processes.Length, finished);
        }

        [Fact]
        public void ForAllAsync_Task_ThreadCountCorrect()
        {
            const int parallelism = 3;
            var processes = new object[30];

            var running = 0;
            var maxThreads = 0;

            var task = processes.AsParallel().ForAllAsync(item => Task.Run(async () =>
            {
                await Task.Delay(delay);
                Interlocked.Increment(ref running);
                await Task.Delay(delay);
                Interlocked.Decrement(ref running);
                await Task.Delay(delay);
            }), parallelism);
            while (task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingForActivation)
                maxThreads = maxThreads > running ? maxThreads : running;

            Assert.Equal(parallelism, maxThreads);
        }

        [Fact]
        public void ForAllAsync_Task_FinishAllOnWait()
        {
            const int parallelism = 3;
            var processes = new object[10];

            var finished = 0;

            processes.AsParallel().ForAllAsync(item => Task.Run(async () =>
            {
                await Task.Delay(delay);
                Interlocked.Increment(ref finished);
            }), parallelism).Wait();

            Assert.Equal(processes.Length, finished);
        }

        [Fact]
        public void ForAllAsync_GreaterProcessCount_ThreadCountEqualToParallelism()
        {
            const int parallelism = 3;
            var processes = new object[20];

            var running = 0;
            var maxThreads = 0;

            var task = processes.AsParallel().ForAllAsync(async item =>
            {
                await Task.Delay(delay);
                Interlocked.Increment(ref running);
                await Task.Delay(delay);
                Interlocked.Decrement(ref running);
                await Task.Delay(delay);
            }, parallelism);
            while (task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingForActivation)
                maxThreads = maxThreads > running ? maxThreads : running;

            Assert.Equal(parallelism, maxThreads);
        }

        [Fact]
        public void ForAllAsync_GreaterParallelism_ThreadCountEqualToProcessCount()
        {
            const int parallelism = 100;
            var processes = new object[3];

            var running = 0;
            var maxThreads = 0;

            var task = processes.AsParallel().ForAllAsync(async item =>
            {
                await Task.Delay(delay);
                Interlocked.Increment(ref running);
                await Task.Delay(delay);
                Interlocked.Decrement(ref running);
                await Task.Delay(delay);
            }, parallelism);
            while (task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingForActivation)
                maxThreads = maxThreads > running ? maxThreads : running;

            Assert.Equal(processes.Length, maxThreads);
        }

        [Fact]
        public void ForAllAsync_EqualParallelismAndProcesses_EqualThreadCount()
        {
            const int parallelism = 10;
            var processes = new object[10];

            var running = 0;
            var maxThreads = 0;

            var task = processes.AsParallel().ForAllAsync(async item =>
            {
                await Task.Delay(delay * 3);
                Interlocked.Increment(ref running);
                await Task.Delay(delay * 3);
                Interlocked.Decrement(ref running);
                await Task.Delay(delay * 3);
            }, parallelism);
            while (task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingForActivation)
                maxThreads = maxThreads > running ? maxThreads : running;

            Assert.Equal(processes.Length, maxThreads);
            Assert.Equal(parallelism, maxThreads);
        }
    }
}