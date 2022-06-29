using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class StringDateTimeExtensionTests : xUnitTestBase<StringDateTimeExtensionTests>
    {
        public StringDateTimeExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void IsValidDateTimeReturnsBooleanValue()
        {
            Assert.True("01/01/2020".IsValidDateTime("en-US"));
        }
    }
}