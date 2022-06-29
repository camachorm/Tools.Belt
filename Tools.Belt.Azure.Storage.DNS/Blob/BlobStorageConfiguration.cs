using System;
using Tools.Belt.Azure.Storage.DNS.Extensions;
using Tools.Belt.Common.Abstractions.Configuration.Entities;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public class BlobStorageConfiguration : DefaultAzureActiveDirectoryClientCredentialsConfiguration,
        IBlobStorageConfiguration
    {
        private const string AccountNameSettings = "AccountName";

        public BlobStorageConfiguration(ISystemConfiguration configuration) : base(configuration)
        {
        }

        /// <inheritdoc/>
        public Uri StorageUri => StorageAccountName.ToBlobStorageAccountUri();

        /// <inheritdoc/>
        public string StorageAccountName => ConfigurationService[AccountNameSettings];
    }
}