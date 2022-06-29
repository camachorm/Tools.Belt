using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace Tools.Belt.Common.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        ///     Adds the specified parameter to the Query String.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramName">Name of the parameter to add.</param>
        /// <param name="paramValue">Value for the parameter to add.</param>
        /// <param name="includeIfValueIsNullOrEmpty">
        ///     If this parameter is true and <see cref="paramValue" /> is null or Empty,
        ///     then it just returns the unchanged <see cref="url" />.
        /// </param>
        /// <returns>Url with added parameter.</returns>
        public static Uri AddParameter(this Uri url, string paramName, string paramValue,
            bool includeIfValueIsNullOrEmpty = true)
        {
            if (!includeIfValueIsNullOrEmpty && paramValue.IsNullOrEmpty()) return url;
            UriBuilder uriBuilder = new UriBuilder(url);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[paramName] = paramValue;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        ///     Append the given query key and value to the URI.
        /// </summary>
        /// <param name="uri">The base URI.</param>
        /// <param name="name">The name of the query key.</param>
        /// <param name="value">The query value.</param>
        /// <returns>The combined result.</returns>
        public static Uri AddQueryString(this Uri uri, string name, string value)
        {
            if (value.IsNullOrEmpty()) return uri;

            var uriString = uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
            return new Uri(AddQueryString(uriString, name, value), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        ///     Append the given query key and value to the URI.
        /// </summary>
        /// <param name="uri">The base URI.</param>
        /// <param name="name">The name of the query key.</param>
        /// <param name="value">The query value.</param>
        /// <returns>The combined result.</returns>
        public static string AddQueryString(this string uri, string name, string value)
        {
            if (value.IsNullOrEmpty()) return uri;

            return QueryHelpers.AddQueryString(uri, name, value);
        }

        /// <summary>
        ///     Append the given query keys and values to the URI.
        /// </summary>
        /// <param name="uri">The base URI.</param>
        /// <param name="queryStringValues">A dictionary of query keys and values to append.</param>
        /// <returns>The combined result.</returns>
        public static Uri AddQueryString(this Uri uri, IDictionary<string, string> queryStringValues)
        {
            if (!queryStringValues.Any()) return uri;

            var uriString = uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
            return new Uri(QueryHelpers.AddQueryString(uriString, queryStringValues), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        ///     Append the given query keys and values to the URI.
        /// </summary>
        /// <param name="uri">The base URI.</param>
        /// <param name="queryStringValues">A dictionary of query keys and values to append.</param>
        /// <returns>The combined result.</returns>
        public static string AddQueryString(this string uri, IDictionary<string, string> queryStringValues)
        {
            if (!queryStringValues.Any()) return uri;
            
            return QueryHelpers.AddQueryString(uri, queryStringValues);
        }
    }
}