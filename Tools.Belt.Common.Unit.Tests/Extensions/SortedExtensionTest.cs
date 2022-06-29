using System.Collections.Generic;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class SortedExtensionTests : xUnitTestBase<SortedExtensionTests>
    {
        public SortedExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ToSortedSetWithOrderedSourceSucceeds()
        {
            var source = new List<int> {0, 1, 2};

            Assert.Equal(source, source.ToSortedSet());
        }

        [Fact]
        public void ToSortedSetWithUnOrderedSourceFails()
        {
            var source = new List<int> {0, 3, 2};

            Assert.NotEqual(source, source.ToSortedSet());
        }
    }
}