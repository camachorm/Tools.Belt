using System;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class FileSizeExtensionTests : xUnitTestBase<FileSizeExtensionTests>
    {
        private const double KB = 1024;

        public FileSizeExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void GetBytesReadable1BSucceeds()
        {
            Assert.Equal("1 B", FileSizeExtensions.GetBytesReadable(1));
        }

        [Fact]
        public void GetBytesReadable1KBSucceeds()
        {
            Assert.Equal("1 KB", long.Parse(Math.Pow(KB, 1).ToString()).GetBytesReadable());
        }

        [Fact]
        public void GetBytesReadable1MBSucceeds()
        {
            Assert.Equal("1 MB", long.Parse(Math.Pow(KB, 2).ToString()).GetBytesReadable());
        }

        [Fact]
        public void GetBytesReadable1GBSucceeds()
        {
            Assert.Equal("1 GB", long.Parse(Math.Pow(KB, 3).ToString()).GetBytesReadable());
        }

        [Fact]
        public void GetBytesReadable1TBSucceeds()
        {
            Assert.Equal("1 TB", long.Parse(Math.Pow(KB, 4).ToString()).GetBytesReadable());
        }

        [Fact]
        public void GetBytesReadable1PBSucceeds()
        {
            Assert.Equal("1 PB", long.Parse(Math.Pow(KB, 5).ToString()).GetBytesReadable());
        }

        [Fact]
        public void GetBytesReadable1TEbSucceeds()
        {
            Assert.Equal("1 EB", 0x1000000000000000.GetBytesReadable());
        }
    }
}