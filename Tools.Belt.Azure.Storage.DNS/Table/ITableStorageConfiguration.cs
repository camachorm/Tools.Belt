using Tools.Belt.Azure.DNS.Abstractions.Configuration.AzureActiveDirectory;
using Tools.Belt.Common.Abstractions.Configuration.Entities;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    public interface ITableStorageConfiguration : IAzureActiveDirectoryClientCredentialsConfiguration,
        IConfigurationEntityBase
    {
        /// <summary>
        ///     The name of the table we want to connect to.
        /// </summary>
        string TableName { get; }

        /// <summary>
        ///     The Connection String to access the storage accountWrapper.
        /// </summary>
        string StorageAccountConnectionString { get; }

        /// <summary>
        ///     System wide setting that defines if we should use the CreateIfNotExist function of
        /// </summary>
        bool CreateIfNotExist { get; }

        /// <summary>
        ///     Defines the maximum batch size for a table operation.
        /// </summary>
        int TableStorageBatchSizeThreshold { get; }

        /// <summary>
        ///     Defines the maximum amount of parallel storage queries when retrieving existing records.
        /// </summary>
        int TableStorageMaxParallelRetrieve { get;  }
    }
}