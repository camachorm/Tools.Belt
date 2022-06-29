using System;
using System.Collections.Generic;
using Tools.Belt.Common.Extensions;
using Xunit;

namespace Tools.Belt.Common.Unit.Tests.Extensions
{
    public class UriExtensionTests
    {
        [Fact]
        public void AddParameterSucceeds()
        {
            var baseUrl = new Uri("http://www.microsoft.com");
            var completeUrl = new Uri("http://www.microsoft.com?query=1");

            Assert.Equal(completeUrl, baseUrl.AddParameter("query", "1"));
            Assert.Equal(baseUrl, baseUrl.AddParameter("", "", false));
        }

        [Fact]
        public void AddQueryStringToAbsoluteUriSucceeds()
        {
            var baseUrl = new Uri("http://www.microsoft.com");
            var completeUrl = new Uri("http://www.microsoft.com?query=1");
            
            Assert.Equal(completeUrl, baseUrl.AddQueryString("query", "1"));
            Assert.Equal(baseUrl, baseUrl.AddQueryString("", ""));

            var queryStringValues = new Dictionary<string, string>()
            {
                {"query", "1"}
            };

            Assert.Equal(completeUrl, baseUrl.AddQueryString(queryStringValues));

            queryStringValues.Clear();
            Assert.Equal(baseUrl, baseUrl.AddQueryString(queryStringValues));
        }

        [Fact]
        public void AddQueryStringToRelativeUriSucceeds()
        {
            var baseUrl = new Uri("api/test", UriKind.Relative);
            var completeUrl = new Uri("api/test?query=1", UriKind.Relative);

            Assert.Equal(completeUrl, baseUrl.AddQueryString("query", "1"));
            Assert.Equal(baseUrl, baseUrl.AddQueryString("", ""));

            var queryStringValues = new Dictionary<string, string>()
            {
                {"query", "1"}
            };

            Assert.Equal(completeUrl, baseUrl.AddQueryString(queryStringValues));

            queryStringValues.Clear();
            Assert.Equal(baseUrl, baseUrl.AddQueryString(queryStringValues));
        }
    }
}