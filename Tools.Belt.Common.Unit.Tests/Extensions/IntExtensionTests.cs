using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class IntExtensionTests
    {
        [Fact]
        public void IsIsSuccessfulStatusCodeSucceeds()
        {
            Assert.True(200.IsSuccessfulStatusCode());
            Assert.False(404.IsSuccessfulStatusCode());
        }

        [Fact]
        public void LimitSucceeds()
        {
            Assert.Equal(200, 200.Limit(100, 300));
        }
    }
}