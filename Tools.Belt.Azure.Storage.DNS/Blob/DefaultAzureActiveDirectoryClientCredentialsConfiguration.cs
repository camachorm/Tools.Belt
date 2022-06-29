using Tools.Belt.Azure.DNS.Abstractions.Configuration.AzureActiveDirectory;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Abstractions.Configuration.Entities;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public class DefaultAzureActiveDirectoryClientCredentialsConfiguration : IAzureActiveDirectoryClientCredentialsConfiguration
    {
        public const string TenantIdSettingName = "TenantId";
        public const string SubscriptionIdSettingName = "SubscriptionId";
        public const string ClientIdSettingName = "AadClientId";
        public const string ClientSecretSettingName = "AadClientSecret";

        public DefaultAzureActiveDirectoryClientCredentialsConfiguration(ISystemConfiguration configuration)
        {
            SystemConfiguration = configuration;
            ConfigurationService = configuration.ConfigurationService;
        }

        public string TenantId => ConfigurationService[TenantIdSettingName];
        public string SubscriptionId => ConfigurationService[SubscriptionIdSettingName];
        public string ClientId => ConfigurationService[ClientIdSettingName];
        public string ClientSecret => ConfigurationService[ClientSecretSettingName];

        public IConfigurationService ConfigurationService { get; }
        public ISystemConfiguration SystemConfiguration { get; }
    }
}