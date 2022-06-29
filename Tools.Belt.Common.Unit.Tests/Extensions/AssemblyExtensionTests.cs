using System;
using System.Linq;
using System.Reflection;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Tests.xUnit;
using Xunit;
using Xunit.Abstractions;
using AssemblyExtensions = Tools.Belt.Common.Extensions.AssemblyExtensions;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class AssemblyExtensionTests : xUnitTestBase<AssemblyExtensionTests>
    {
        public AssemblyExtensionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ReadEmbeddedResourceWithNullSourceFails()
        {
            Assembly assembly = null; //typeof(AssemblyExtensionTests).Assembly;
            Assert.Throws<ArgumentNullException>(() => assembly.ReadEmbeddedResource("TestResourceName"));
        }

        [Fact]
        public void ReadEmbeddedResourceWithNullResourceNameFails()
        {
            var assembly = typeof(AssemblyExtensions).Assembly;
            Assert.Throws<ArgumentNullException>(() => assembly.ReadEmbeddedResource(string.Empty));
        }

        [Fact]
        public void ReadEmbeddedResourceWithInvalidResourceNameFails()
        {
            var assembly = typeof(AssemblyExtensions).Assembly;
            Assert.Throws<ArgumentException>(() => assembly.ReadEmbeddedResource("Fake Resource Name"));
        }

        [Fact]
        public void ReadEmbeddedResourceSucceeds()
        {
            var assembly = typeof(AssemblyExtensions).Assembly;
            var resourceName = assembly.GetManifestResourceNames().First();
            Assert.False(assembly.ReadEmbeddedResource(resourceName).IsNullOrEmpty());
        }
    }
}