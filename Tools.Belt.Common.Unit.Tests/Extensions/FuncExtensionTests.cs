using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class FuncExtensionTests : xUnitTestBase<FuncExtensionTests>
    {
        public FuncExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void WrapTestSucceeds()
        {
            var dict = new Dictionary<string, JToken>();
            var result = new Func<int>(() => { return 1; }).WrapTestOperation("logging", dict, Logger);
            Assert.Equal(1, result);
            Assert.Contains(dict, pair => pair.Key == "logging");
        }

        [Fact]
        public void WrapTestGeneralExceptionFails()
        {
            var keyToLog = "errKey";
            var e = new Exception("Intentionally generated exception");
            var mockedLogger = new Mock<ILogger>();
            var x = new Dictionary<string, JToken>();
            var result = new Func<int>(() => throw e);

            result.WrapTestOperation(keyToLog, x, mockedLogger.Object);

            Assert.Equal(keyToLog,x.Keys.Flatten());
        }

        [Fact]
        public void WrapTestOperationWithNoKeyFails()
        {
            var dict = new Dictionary<string, JToken>();
            var result = new Func<int?>(() => { return 1; });

            Assert.Throws<ArgumentNullException>(() => result.WrapTestOperation(null, dict, Logger));
        }

        [Fact]
        public void WrapTestOperationWithNullActionFails()
        {
            var dict = new Dictionary<string, JToken>();
            Func<bool?> xFunc = null;
            Assert.Throws<ArgumentNullException>(() => xFunc.WrapTestOperation("logging", dict, Logger));
        }

        [Fact]
        public void WrapTestOperationWithNullLogFails()
        {
            var dict = new Dictionary<string, JToken>();
            var result = new Func<int?>(() => { return 1; });

            Assert.Throws<ArgumentNullException>(() => result.WrapTestOperation("logging", null, Logger));
        }

        [Fact]
        public async Task WrapTestOperationLogsExceptionAsync()
        {
            var keyToLog = "errKey";
            var e = new Exception("Intentionally generated exception");
            var mockedLogger = new Mock<ILogger>();
            var x = new Mock<IDictionary<string, JToken>>();

            var result = new Func<Task>(() => { throw e; });
            await result.WrapTestOperation(keyToLog, x.Object, mockedLogger.Object);
            x.Verify(dictionary => dictionary.Add(It.Is<string>(s => s == keyToLog), It.IsAny<JToken>()));
        }

        [Fact]
        public async Task WrapTestOperationAsyncWithNullActionFails()
        {
            var keyToLog = "errKey";
            var mockedLogger = new Mock<ILogger>();
            var dictMock = new Mock<IDictionary<string, JToken>>();

            Func<Task> nullFunc = null;

            await Assert.ThrowsAnyAsync<ArgumentNullException>(async () =>
                await nullFunc.WrapTestOperation(keyToLog, dictMock.Object, mockedLogger.Object));
        }

        [Fact]
        public async Task WrapTestOperationAsyncWithNullKeyFails()
        {
            string keyToLog = null;
            var loggerMock = new Mock<ILogger>();
            var dictMock = new Mock<IDictionary<string, JToken>>();

            var testFunc = new Func<Task>(() => Task.CompletedTask);

            await Assert.ThrowsAnyAsync<ArgumentNullException>(async () =>
                await testFunc.WrapTestOperation(keyToLog, dictMock.Object, loggerMock.Object));
        }

        [Fact]
        public async Task WrapTestOperationAsyncWithNullLogFails()
        {
            var keyToLog = "err";
            var loggerMock = new Mock<ILogger>();
            
            var testFunc = new Func<Task>(() => Task.CompletedTask);

            IDictionary<string, JToken> nullLog = null;
            await Assert.ThrowsAnyAsync<ArgumentNullException>(async () =>
                await testFunc.WrapTestOperation(keyToLog, nullLog, loggerMock.Object));
        }

        [Fact]
        public async Task WrapTestOperationAsyncSucceeds()
        {
            var keyToLog = "Result";
            var loggerMock = new Mock<ILogger>();
            var dictMock = new Mock<IDictionary<string, JToken>>();

            var testFunc = new Func<Task>(() => Task.CompletedTask);
            await testFunc.WrapTestOperation(keyToLog, dictMock.Object, loggerMock.Object);

            dictMock.Verify(dictionary => dictionary.Add(It.Is<string>(s => s == keyToLog),
                It.Is<JToken>(t => t.Value<string>() == "Success")));
        }

        [Fact]
        public async Task WrapTestOperationLogsException()
        {
            var keyToLog = "errKey";
            var e = new Exception("Intentionally generated exception");
            var mockedLogger = new Mock<ILogger>();
            var x = new Mock<IDictionary<string, JToken>>();

            var result = new Func<Task>(() => { throw e; });

            await result.WrapTestOperation(keyToLog, x.Object, mockedLogger.Object);

            x.Verify(dictionary => dictionary.Add(It.Is<string>(s => s == keyToLog), It.IsAny<JToken>()));
        }

        [Fact]
        public async Task WrapTestOperationSucceeds()
        {
            var keyToLog = "TestResult";
            var e = new Exception("Intentionally generated exception");
            var mockedLogger = new Mock<ILogger>();
            var x = new Mock<IDictionary<string, JToken>>();

            var result = new Func<Task<int>>(() => throw e);

            await result.WrapTestOperation(keyToLog, x.Object, mockedLogger.Object);
            x.Verify(dictionary => dictionary.Add(It.Is<string>(s => s == keyToLog), It.IsAny<JToken>()));
        }

        [Fact]
        public async Task WrapTestOperationGenericWithNullActionFails()
        {
            var keyToLog = "errKey";
            var mockedLogger = new Mock<ILogger>();
            var dictMock = new Mock<IDictionary<string, JToken>>();

            Func<Task<int>> nullFunc = null;
            await Assert.ThrowsAnyAsync<ArgumentNullException>(async () =>
                await nullFunc.WrapTestOperation(keyToLog, dictMock.Object, mockedLogger.Object));
        }

        [Fact]
        public async Task WrapTestOperationGenericWithNullKeyFails()
        {
            string keyToLog = null;
            var loggerMock = new Mock<ILogger>();
            var dictMock = new Mock<IDictionary<string, JToken>>();

            var testFunc = new Func<Task<bool>>(() => Task.FromResult(true));
            await Assert.ThrowsAnyAsync<ArgumentNullException>(async () =>
                await testFunc.WrapTestOperation(keyToLog, dictMock.Object, loggerMock.Object));
        }

        [Fact]
        public async Task WrapTestOperationGenericWithNullDictFails()
        {
            var keyToLog = "err";
            var loggerMock = new Mock<ILogger>();
            var dictMock = new Mock<IDictionary<string, JToken>>();

            var testFunc = new Func<Task<int>>(() => Task.FromResult(1));

            IDictionary<string, JToken> nullLog = null;
            await Assert.ThrowsAnyAsync<ArgumentNullException>(async () =>
                await testFunc.WrapTestOperation(keyToLog, nullLog, loggerMock.Object));
        }

        [Fact]
        public async Task WrapTestOperationGenericSucceeds()
        {
            var keyToLog = "Result";
            var loggerMock = new Mock<ILogger>();
            var dictMock = new Mock<IDictionary<string, JToken>>();

            var testFunc = new Func<Task<int>>(() => Task.FromResult(1));
            await testFunc.WrapTestOperation(keyToLog, dictMock.Object, loggerMock.Object);

            dictMock.Verify(dictionary => dictionary.Add(It.Is<string>(s => s == keyToLog), It.IsAny<JToken>()));
        }

        [Fact]
        public async Task TaskWrapAndTimeTestOperationSucceeds()
        {
            var keyToLog = "TestResult";
            var loggerMock = new Mock<ILogger>();
            var testFunc = new Func<Task>(() => Task.CompletedTask);

            await testFunc.WrapAndTimeTestOperationAsync(keyToLog, new Dictionary<string, JToken>(), loggerMock.Object);
        }

        [Fact]
        public async Task TimeOperationSucceeds()
        {
            var keyToLog = "TestResult";
            var loggerMock = new Mock<ILogger>();
            var testFunc = new Func<Task>(() => Task.CompletedTask);
            var dict = new Dictionary<string, JToken>();

            dict.Add(keyToLog, null);
            await testFunc.TimeOperationAsync(keyToLog, dict, loggerMock.Object);
        }

        [Fact]
        public async Task TimeOperationAsyncAddsKeyToLog()
        {
            var keyToLog = "TestResult";
            var loggerMock = new Mock<ILogger>();
            var testFunc = new Func<Task>(() => Task.CompletedTask);
            var dict = new Mock<IDictionary<string, JToken>>();

            await testFunc.TimeOperationAsync(keyToLog, dict.Object, loggerMock.Object);
            dict.Verify(dictionary => dictionary.Add(It.Is<string>(s => s == keyToLog), It.IsAny<JToken>()));
        }

        [Fact]
        public void WrapAndTimeTestOperationSucceeds()
        {
            var keyToLog = "TestResult";
            var testFunc = new Func<Task>(() => Task.CompletedTask);

            var loggerMock = new Mock<ILogger>();
            var dict = new Mock<Dictionary<string, JToken>>();

            testFunc.WrapAndTimeTestOperation(keyToLog, dict.Object, loggerMock.Object);
        }

        [Fact]
        public void TimeTestOperationSucceeds()
        {
            var keyToLog = "TestResult";
            var testFunc = new Func<Task>(() => Task.CompletedTask);

            var loggerMock = new Mock<ILogger>();
            var dict = new Mock<Dictionary<string, JToken>>();

            testFunc.TimeOperation(keyToLog, dict.Object, loggerMock.Object);
        }

        [Fact]
        public async Task TimeOperationAsyncSucceeds()
        {
            var keyToLog = "TestResult";
            var testFunc = new Func<Task>(() => Task.CompletedTask);

            var loggerMock = new Mock<ILogger>();

            await testFunc.TimeOperationAsync(keyToLog, loggerMock.Object);
        }

        [Fact]
        public async Task TimeOperationGenericAsyncSucceeds()
        {
            var keyToLog = "TestResult";
            var testFunc = new Func<Task<int>>(() => Task.FromResult(1));

            var loggerMock = new Mock<ILogger>();

            await testFunc.TimeOperationAsync(keyToLog, loggerMock.Object);
        }

        [Fact]
        public void TimeOperationGenericSucceeds()
        {
            var keyToLog = "TestResult";
            var testFunc = new Func<Task<int>>(() => Task.FromResult(1));
            var dict = new Dictionary<string, JToken>();
            var loggerMock = new Mock<ILogger>();
            dict.Add(keyToLog, null);
            testFunc.TimeOperation(keyToLog, dict, loggerMock.Object);
        }

        [Fact]
        public void TimeOperationGenericWithoutKeySucceeds()
        {
            var keyToLog = "TestResult";
            var testFunc = new Func<Task<int>>(() => Task.FromResult(1));
            var dict = new Dictionary<string, JToken>();
            var loggerMock = new Mock<ILogger>();

            testFunc.TimeOperation(keyToLog, dict, loggerMock.Object);
        }
    }
}