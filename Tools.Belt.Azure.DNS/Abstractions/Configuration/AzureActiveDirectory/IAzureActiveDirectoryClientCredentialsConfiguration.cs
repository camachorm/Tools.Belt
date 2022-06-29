using Tools.Belt.Common.Abstractions.Configuration.Entities;

namespace Tools.Belt.Azure.DNS.Abstractions.Configuration.AzureActiveDirectory
{
    public interface IAzureActiveDirectoryClientCredentialsConfiguration
        : IConfigurationEntityBase,
            ISystemConfigurationHolder
    {
        /// <summary>
        ///     The Id of our Azure Tenant.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     The Id of our Azure Subscription.
        /// </summary>
        string SubscriptionId { get; }

        /// <summary>
        ///     The Id of our Application's identity in Azure Active Directory.
        /// </summary>
        string ClientId { get; }


        /// <summary>
        ///     The secret part of our Application's identity in Azure Active Directory.
        ///     TODO: Investigate potential use case for <see cref="System.Security.SecureString" />
        /// </summary>
        string ClientSecret { get; }
    }
}