using System;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class ByteArrayExtensionTests : xUnitTestBase<ByteArrayExtensionTests>
    {
        public ByteArrayExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void DecodeSuccess()
        {
            byte[] byteArray = {111, 99, 101, 97, 110};
            Assert.Equal("ocean", byteArray.Decode());
        }

        [Fact]
        public void DecodeWithNullEncodingFails()
        {
            byte[] byteArray = {111, 99, 101, 97, 110};
            Assert.Throws<ArgumentNullException>(() => byteArray.Decode(null));
        }

        [Fact]
        public void DecodeWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => ByteArrayExtensions.Decode(null));
        }

        [Fact]
        public void EncodeSuccess()
        {
            byte[] byteArray = {111, 99, 101, 97, 110};
            Assert.Equal(byteArray, "ocean".Encode());
        }

        [Fact]
        public void EncodeWithNullEncodingFails()
        {
            Assert.Throws<ArgumentNullException>(() => "ocean".Encode(null));
        }

        [Fact]
        public void EncodeWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => ByteArrayExtensions.Encode(null));
        }
    }
}