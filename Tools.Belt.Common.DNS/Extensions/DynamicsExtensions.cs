// ReSharper disable UnusedMember.Global

using System;

namespace Tools.Belt.Common.Extensions
{
    public static class DynamicsExtensions
    {
        /// <summary>
        ///     Appends the necessary segments to the base Url to make it a proper Service Url
        /// </summary>
        /// <param name="organizationBaseUrl">
        ///     The dynamics instance root Url
        ///     e.g. - https://somedynamycsdev.crm4.dynamics.com
        /// </param>
        /// <returns>
        ///     The Dynamics service url for use with XRM
        ///     e.g. - https://somedynamicsdev.crm4.dynamics.com/xrmservices/2011/organization.svc/web?SdkClientVersion=8.2
        /// </returns>
        public static Uri GetOrganizationResourceServiceUri(this string organizationBaseUrl)
        {
            return new Uri(
                organizationBaseUrl.AddUriSegment("xrmservices/2011/organization.svc/web?SdkClientVersion=8.2"));
        }

        /// <summary>
        ///     Appends the necessary segments to the base Url to make it a proper Service Url
        /// </summary>
        /// <param name="organizationBaseUrl">
        ///     The dynamics instance root Url
        ///     e.g. - https://somedynamycsdev.crm4.dynamics.com
        /// </param>
        /// <returns>
        ///     The Dynamics service url for use with XRM
        ///     e.g. - https://somedynamicsdev.crm4.dynamics.com/xrmservices/2011/organization.svc/web?SdkClientVersion=8.2
        /// </returns>
        public static Uri GetOrganizationResourceServiceUri(this Uri organizationBaseUrl)
        {
            if (organizationBaseUrl == null) throw new ArgumentNullException(nameof(organizationBaseUrl));
            return organizationBaseUrl.AbsoluteUri.GetOrganizationResourceServiceUri();
        }
    }
}