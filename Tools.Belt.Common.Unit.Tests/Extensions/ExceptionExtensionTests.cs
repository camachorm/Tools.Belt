using System;
using System.Net;
using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class ExceptionExtensionTests
    {
        [Fact]
        public void FullStackTraceWithNullSourceFails()
        {
            Exception nullException = null;
            Assert.Throws<ArgumentNullException>(() => nullException.FullStackTrace());
        }

        [Fact]
        public void FullStackTraceWitInnerExceptionSucceeds()
        {
            var excWithInner = new Exception("message", new Exception("inner message"));
            var simpleException = new Exception("message");

            Assert.Contains("Inner Exception:", excWithInner.FullStackTrace());
            Assert.DoesNotContain("Inner Exception:", simpleException.FullStackTrace());
        }

        [Fact]
        public void ToErrorJTokenWithNullSourceFails()
        {
            Exception nullException = null;
            Assert.Throws<ArgumentNullException>(() => nullException.ToErrorJToken());
        }
        
        [Fact]
        public void ToErrorJTokenSucceeds()
        {
            const string exceptionMessage = "Custom Error Message";
            Exception customException = new Exception(exceptionMessage);
            var JTokenError = customException.ToErrorJToken();
            Assert.Equal(exceptionMessage, JTokenError["Message"].SafeToString());
        }

        [Fact]
        public void ProcessWebExceptionWithNonWebExceptionFails()
        {
            var nullException = new WebException();
            Assert.Contains("Web Exception Details:", nullException.FullStackTrace());
        }
    }
}