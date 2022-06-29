using System;
using Tools.Belt.Azure.DNS.Abstractions.Configuration.AzureActiveDirectory;
using Tools.Belt.Common.Abstractions.Configuration.Entities;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public interface IBlobStorageConfiguration
        : IAzureActiveDirectoryClientCredentialsConfiguration,
            IConfigurationEntityBase,
            ISystemConfigurationHolder
    {
        /// <summary>
        /// Reflects the <see cref="Uri"/> of the storage account
        /// </summary>
        Uri StorageUri { get; } 
        
        /// <summary>
        /// Reflects the name of the storage account to connect to.
        /// </summary>
        string StorageAccountName { get; }
    }
}