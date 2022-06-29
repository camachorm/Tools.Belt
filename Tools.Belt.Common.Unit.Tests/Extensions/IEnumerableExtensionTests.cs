using System;
using System.Collections.Generic;
using System.Linq;
using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class IEnumerableExtensionTests
    {
        [Fact]
        public void ChunkSucceeds()
        {
            var longList = Enumerable.Repeat(new TestClass(), 100);
            var chuckedList = longList.Chunk(50);

            Assert.Equal(2, chuckedList.Count());
            foreach (var innerList in chuckedList) Assert.Equal(50, innerList.Count());
        }

        [Fact]
        public void SplitSucceeds()
        {
            var longList = Enumerable.Repeat(new TestClass(), 100);
            var splitList = longList.Split(5);

            Assert.Equal(20, splitList.Count());
            foreach (var innerList in splitList) Assert.Equal(5, innerList.Count());
        }

        [Fact]
        public void SplitWithNullSourceFails()
        {
            IEnumerable<string> nullObject = null;
            Assert.Throws<ArgumentNullException>(() => nullObject.Split().ToList());
        }


        private class TestClass
        {
        }
    }
}