using System;
using System.Text;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class DateTimeExtensionTests : xUnitTestBase<DateTimeExtensionTests>
    {
        public DateTimeExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        [Fact]
        public void IsWeekendOnSaturdaySucceeds()
        {
            Assert.True(new DateTime(2020, 11, 28).IsWeekend());
        }

        [Fact]
        public void IsWeekendOnMondayFails()
        {
            Assert.False(new DateTime(2020, 12, 1).IsWeekend());
        }

        [Fact]
        public void FromBase64Succeeds()
        {
            var testDate = new DateTime(2010, 1, 1);
            var encodedDate = testDate.ToBase64();// "MDEvMDEvMjAxMA=="; 
            
            Assert.Equal(testDate,DateTimeExtensions.FromBase64(encodedDate));
        }

        [Fact]
        public void FromBase64InvalidSourceFails()
        {
            var encodedDate = "b2NlYW5taW5k"; // oceanmind
            Assert.Equal(DateTime.Now.ToShortTimeString(),
                DateTimeExtensions.FromBase64(encodedDate)
                    .ToShortTimeString()); //Equal fails when comparing with now.date
        }

        [Fact]
        public void ToBase64Succeeds()
        {
            var sourceDateTime = DateTime.Now;
            var base64SourceDate =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(sourceDateTime.ToUniversalTime()
                    .ToString("yyyy-MM-dd HH:mm:ss.ffffff+00:00")));

            Assert.Equal(base64SourceDate, sourceDateTime.ToBase64());
        }
    }
}