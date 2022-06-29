using System;
using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class DynamicExtensionTests
    {
        [Fact]
        public void GetOrganizationResourceServiceUriWitStringSucceeds()
        {
            var baseURI = "https://azure.microsoft.com/";
            var expectedUri = new Uri(baseURI + "xrmservices/2011/organization.svc/web?SdkClientVersion=8.2");

            Assert.Equal(expectedUri, baseURI.GetOrganizationResourceServiceUri());
        }


        [Fact]
        public void GetOrganizationResourceServiceUriWitUriSucceeds()
        {
            var organizationUri =
                new Uri(" https://azure.microsoft.com/test/xrmservices/2011/organization.svc/web?SdkClientVersion=8.2");
            var baseUri = new Uri("https://azure.microsoft.com/test");
            Uri nullUri = null;

            Assert.Throws<ArgumentNullException>(() => nullUri.GetOrganizationResourceServiceUri());
            Assert.Equal(organizationUri, baseUri.GetOrganizationResourceServiceUri());
        }
    }
}