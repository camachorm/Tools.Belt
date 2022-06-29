using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class ActionExtensionTests : xUnitTestBase<FuncExtensionTests>
    {
        public ActionExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void WrapTestOperationWithNullActionFails()
        {            
            var keyToLog = "Result";
            var log = new Dictionary<string, JToken>();
            Action testAction = null;

            Assert.Throws<ArgumentNullException>(() => testAction.WrapTestOperation(keyToLog, log));
        }

        [Fact]
        public void WrapTestOperationSucceeds()
        {
            var keyToLog = "Result";
            var log = new Dictionary<string, JToken>();
            Action testAction = () => Console.WriteLine("Works!");

            testAction.WrapTestOperation(keyToLog, log);
        }

        [Fact]
        public void WrapTestOperationWithILoggerNullActionFails()
        {
            var keyToLog = "Result";
            var loggerMock = new Mock<ILogger>();

            Action testAction = null;
            Assert.Throws<ArgumentNullException>(() => testAction.WrapTestOperation(keyToLog, loggerMock.Object));
        }

        [Fact]
        public void WrapTestOperationWithNullActionsFails()
        {
            var keyToLog = "Result";

            Action<string, string> nullAction = null;
            Action testAction = null;

            Assert.Throws<ArgumentNullException>(() => testAction.WrapTestOperation(keyToLog, nullAction, nullAction));
        }

        [Fact]
        public void WrapTestOperationWithErrorActionFails()
        {
            var keyToLog = "Result";
            Action<string, string> nullAction = null;

            var errAction = new Action(() => { throw new Exception(); });

            Assert.Throws<NullReferenceException>(() => errAction.WrapTestOperation(keyToLog, nullAction, nullAction));
        }

        [Fact]
        public void WrapTestOperationWithILoggerSucceeds()
        {
            var keyToLog = "Result";
            var loggerMock = new Mock<ILogger>();
            Action testAction = () => Console.WriteLine("Works!");

            testAction.WrapTestOperation(keyToLog, loggerMock.Object);
        }

        [Fact]
        public void WrapAndTimeSucceeds()
        {
            var keyToLog = "Result";
            Action testAction = () => Console.WriteLine("Works!");

            void TestActionString(string s)
            {
                Console.WriteLine(s);
            }

            testAction.WrapAndTimeTestOperation(keyToLog, TestActionString);
        }

        [Fact]
        public void WrapAndTimeWithDictionarySucceeds()
        {
            var keyToLog = "Result";
            var log = new Dictionary<string, JToken>();
            Action testAction = () => Console.WriteLine("Works!");

            testAction.WrapAndTimeTestOperation(keyToLog, log);
        }

        [Fact]
        public void TimeOperationWithDictionarySucceeds()
        {
            var keyToLog = "Result";
            var log = new Dictionary<string, JToken>();
            Action testAction = () => Console.WriteLine("Works!");

            testAction.TimeOperation(keyToLog, log);
        }

        [Fact]
        public void TimeOperationWithDictionaryAndNullActionFails()
        {
            var keyToLog = "Result";
            var log = new Dictionary<string, JToken>();
            Action testAction = null;

            Assert.Throws<ArgumentNullException>(() => testAction.TimeOperation(keyToLog, log));
        }

        [Fact]
        public void TimeOperationWithDictionaryAndNullLogFails()
        {
            var keyToLog = "Result";
            Dictionary<string, JToken> log = null;
            Action testAction = () => Console.WriteLine("Works!");

            Assert.Throws<ArgumentNullException>(() => testAction.TimeOperation(keyToLog, log));
        }

        [Fact]
        public void TimeOperationWithNullActionFails()
        {
            var keyToLog = "Result";
            var loggerMock = new Mock<ILogger>();

            Action testAction = null;
            Assert.Throws<ArgumentNullException>(() => testAction.TimeOperation(keyToLog, loggerMock.Object));
        }

        [Fact]
        public void TimeOperationWithNullLogFails()
        {
            var keyToLog = "Result";

            Action testAction = () => Console.WriteLine("Works!");

            Action<string> testActionString = null;

            Assert.Throws<ArgumentNullException>(() => testAction.TimeOperation(keyToLog, testActionString));
        }

        [Fact]
        public void TimeOperationSucceeds()
        {
            var keyToLog = "Result";
            var loggerMock = new Mock<ILogger>();
            Action testAction = () => Console.WriteLine("Works!");

            testAction.TimeOperation(keyToLog, loggerMock.Object);
        }
    }
}