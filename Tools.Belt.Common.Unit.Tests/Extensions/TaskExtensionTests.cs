using System;
using System.Threading;
using System.Threading.Tasks;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;
using TaskExtensions = Tools.Belt.Common.Extensions.TaskExtensions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class TaskExtensionTests : xUnitTestBase<TaskExtensionTests>
    {
        public TaskExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void RunSyncRunSynchronouslyWithNullSourceFails()
        {
            Task<bool> t = null;
            Assert.Throws<ArgumentNullException>(() =>
                TaskExtensions.ReturnAsyncCallSynchronously(t, int.MaxValue, CancellationToken.None));
        }

        [Fact]
        public void RunSyncRunSynchronouslySucceeds()
        {
            var x = Task.Run(() => true);
            Assert.True(TaskExtensions.ReturnAsyncCallSynchronously(x));
        }

        [Fact]
        public void RunSyncRunSynchronouslyTimesOut()
        {
            var x = Task.Run(() =>
            {
                Thread.Sleep(20000);
                return true;
            });

            Assert.Throws<TimeoutException>(() =>
                TaskExtensions.ReturnAsyncCallSynchronously(x, 10, CancellationToken.None));
        }
    }
}