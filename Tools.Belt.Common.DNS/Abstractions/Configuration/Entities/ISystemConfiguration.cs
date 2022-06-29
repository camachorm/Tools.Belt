namespace Tools.Belt.Common.Abstractions.Configuration.Entities
{
    public interface ISystemConfiguration : IConfigurationEntityBase
    {
        /// <summary>
        ///     Flag used to determine if debug only operations are executed. For those consuming tasks that are needed during
        ///     investigation.
        /// </summary>
        bool SystemDebug { get; }

        /// <summary>
        ///     TODO: Eduard - please detail the purpose of this variable here
        /// </summary>
        int QueueParallelism { get; }

        /// <summary>
        ///     The maximum size in bytes for a service bus message object. Used to break down batches of messages in to valid
        ///     chunks.
        /// </summary>
        int MaximumMessageSize { get; }

        /// <summary>
        ///     The maximum number of Retries to be performed before failing an operation wrapped in Polly.
        /// </summary>
        int PollyRetryLimit { get; }

        /// <summary>
        ///     The number of milliseconds to wait before retrying a failed operation wrapped in Polly.
        /// </summary>
        int PollyRetryWaitMs { get; }

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
        int TableStorageMaxParallelRetrieve { get; }
    }
}