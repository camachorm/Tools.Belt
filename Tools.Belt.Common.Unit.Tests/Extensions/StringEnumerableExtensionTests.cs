using System.Collections.Generic;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class StringEnumerableExtensionTests : xUnitTestBase<StringEnumerableExtensionTests>
    {
        public StringEnumerableExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void FlatternBreaksListElementsInNewLines()
        {
            Assert.Equal("ocean\r\nmind", new List<string> {"ocean", "mind"}.Flatten());
        }

        [Fact]
        public void ToCSVAddsSeparator()
        {
            Assert.Equal(",ocean,mind,Ltd.", new List<string> {"ocean", "mind", "Ltd."}.ToCsv());
        }
    }
}