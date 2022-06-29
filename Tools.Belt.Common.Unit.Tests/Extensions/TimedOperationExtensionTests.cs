using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class TimedOperationExtensionTests : xUnitTestBase<TimedOperationExtensionTests>
    {
        public TimedOperationExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void TimeOperationWithNullActionFails()
        {
            Action nullAction = null;
            var mockLogger = new Mock<ILogger>();
            Assert.Throws<ArgumentNullException>(() => nullAction.TimeOperation(mockLogger.Object));
        }

        [Fact]
        public void TimeOperationWithTimerSucceeds()
        {
            Action testAction = delegate { };
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(logger => logger.IsEnabled(It.Is<LogLevel>(level => level == LogLevel.Trace)))
                .Returns(true);

            testAction.TimeOperation(mockLogger.Object);
        }

        [Fact]
        public void TimeOperationSucceeds()
        {
            var mockLogger = new Mock<ILogger>();
            Action testAction = delegate { };
            testAction.TimeOperation(mockLogger.Object);
        }

        [Fact]
        public async Task TimeAsyncOperationWithNullActionFails()
        {
            var mockLogger = new Mock<ILogger>();
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                ((Func<Task>) null).TimeAsyncOperation(mockLogger.Object));
        }

        [Fact]
        public async Task TimeAsyncOperationGenericWithNullActionFails()
        {
            var mockLogger = new Mock<ILogger>();
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                ((Func<Task<int>>) null).TimeAsyncOperation(mockLogger.Object));
        }

        [Fact]
        public async Task TimeAsyncGenericOperationSucceeds()
        {
            var mockLogger = new Mock<ILogger>();
            var testFunc = new Func<Task<bool>>(() => Task.FromResult(true));
            var returnValue = await testFunc.TimeAsyncOperation(mockLogger.Object);
            Assert.True(returnValue);
        }

        [Fact]
        public async Task TimeAsyncGenericOperationWithTimerSucceeds()
        {
            var mockLogger = new Mock<ILogger>();
            var testFunc = new Func<Task<bool>>(() => Task.FromResult(true));
            mockLogger.Setup(logger => logger.IsEnabled(It.Is<LogLevel>(level => level == LogLevel.Trace)))
                .Returns(true);
            var returnValue = await testFunc.TimeAsyncOperation(mockLogger.Object);
            Assert.True(returnValue);
        }

        [Fact]
        public async Task TimeAsyncOperationSucceeds()
        {
            var mockLogger = new Mock<ILogger>();
            var testFunc = new Func<Task>(() => Task.FromResult(true));
            await testFunc.TimeAsyncOperation(mockLogger.Object);
        }

        [Fact]
        public async Task TimeAsyncOperationWithTimerSucceeds()
        {
            var mockLogger = new Mock<ILogger>();
            var testFunc = new Func<Task>(() => Task.FromResult(true));
            mockLogger.Setup(logger => logger.IsEnabled(It.Is<LogLevel>(level => level == LogLevel.Trace)))
                .Returns(true);
            await testFunc.TimeAsyncOperation(mockLogger.Object);
        }

        [Fact]
        public void StopTimerSucceeds()
        {
            var mockLogger = new Mock<ILogger>().Object;
            var operationName = "";
            TimedOperationsExtensions.StopTimer(operationName, mockLogger);
        }

        [Fact]
        public void StartTimerSucceeds()
        {
            var mockLogger = new Mock<ILogger>().Object;
            var operationName = "";
            TimedOperationsExtensions.StartTimer(operationName, mockLogger);
        }
    }
}