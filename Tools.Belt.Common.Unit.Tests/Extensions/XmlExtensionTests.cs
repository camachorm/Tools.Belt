using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class XmlExtensionTests
    {
        [Fact]
        public void TryParseSucceeds()
        {
            var xmlSourceString = "<item type='myType'>this is my value</item>";
            Assert.True(xmlSourceString.TryParse(out var outputXElement));
            Assert.False("".TryParse(out var emptyXElement));
        }
    }
}