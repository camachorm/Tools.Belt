using System;
using System.Linq;
using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class TypeExtensionTests
    {
        [Fact]
        public void ReadEmbeddedResourceWithNullSourceFails()
        {
            Assert.Throws<ArgumentNullException>(() => TypeExtensions.ReadEmbeddedResource(null, "resType"));
        }

        [Fact]
        public void ReadEmbeddedResourceSucceeds()
        {
            var assembly = typeof(AssemblyExtensions).Assembly;
            var assemblyType = typeof(AssemblyExtensions);

            var resourceName = assembly.GetManifestResourceNames().First();

            Assert.False(assemblyType.ReadEmbeddedResource(resourceName).IsNullOrEmpty());
        }
    }
}