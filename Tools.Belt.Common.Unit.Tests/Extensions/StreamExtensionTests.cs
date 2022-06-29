using System.IO;
using System.Text;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class StreamExtensionTests : xUnitTestBase<StreamExtensionTests>
    {
        public StreamExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ReadToEndSucceeds()
        {
            var sourceString = "oceanmind";
            var byteArray = Encoding.ASCII.GetBytes(sourceString);
            var stream = new MemoryStream(byteArray);

            Assert.Equal(sourceString, stream.ReadToEnd());
        }
    }
}