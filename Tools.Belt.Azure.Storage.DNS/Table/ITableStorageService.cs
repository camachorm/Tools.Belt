using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools.Belt.Azure.Storage.DNS.Common;
using Tools.Belt.Azure.Storage.DNS.Table.Structures.Query;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    /// <summary>
    ///     Determines the shared public signature of all derived types.
    /// </summary>
    public interface ITableStorageService : IStorageService
    {
        ITableStorageConfiguration ConfigurationService {  get; }

        /// <summary>
        ///     Provides access to the underlying <see cref="ICloudTableClientWrapper" />.
        /// </summary>
        ICloudTableClientWrapper StorageClient { get; }

        Task<ICloudTableWrapper> GetCloudTableReference();

        /// <summary>
        ///     Utility method that creates table if it doesn't exist already.
        /// </summary>
        Task CreateTableIfNotExistsAsync();

        /// <summary>
        ///     Checks if a given record exists in the table storage synchronously.
        /// </summary>
        /// <param name="record">The <see cref="ITableEntity" /> record.</param>
        bool Exists<T>(T record) where T : ITableEntity, new();

        /// <summary>
        ///     Checks if a given list of records exists in the table storage synchronously.
        /// </summary>
        /// <param name="records">The <see cref="ITableEntity" /> records.</param>
        bool Exists<T>(IEnumerable<T> records) where T : ITableEntity, new();

        /// <summary>
        ///     Checks if a given list of records exists in the table storage synchronously.
        /// </summary>
        /// <param name="records">The <see cref="ITableEntity" /> records.</param>
        Task<bool> ExistsAsync<T>(IEnumerable<T> records) where T : ITableEntity, new();

        /// <summary>
        ///     Checks if a given record exists in the table storage asynchronously.
        /// </summary>
        /// <param name="record">The <see cref="ITableEntity" /> record.</param>
        Task<bool> ExistsAsync<T>(T record) where T : ITableEntity, new();

        /// <summary>
        ///     Returns a list of all the existing records that match the PartitionKey/RowKey of the provided records.
        ///     Runs queries in parallel.
        /// </summary>
        /// <param name="records">Records to retrieve if they exist.</param>
        /// <param name="selectColumns">Columns to retrieve (optional, all by default).</param>
        /// <param name="tableName">Table name to query (optional, default AIS/main table).</param>
        Task<IList<T>> GetExistingAsync<T>(IEnumerable<T> records, List<string> selectColumns = null, string tableName = null) where T : ITableEntity, new();

        /// <summary>
        ///     Adds a new record to the table storage synchronously.
        /// </summary>
        /// <param name="record">The <see cref="ITableEntity" /> record.</param>
        void AddRecord<T>(T record) where T: ITableEntity, new();

        /// <summary>
        ///     Adds a new record to the table storage asynchronously.
        /// </summary>
        /// <param name="record">The <see cref="ITableEntity" /> record.</param>
        Task AddRecordAsync<T>(T record) where T: ITableEntity, new();

        /// <summary>
        ///     Adds or replaces a record to the table storage asynchronously.
        /// </summary>
        /// <param name="record">The <see cref="ITableEntity" /> record.</param>
        Task AddOrReplaceRecordAsync<T>(T record) where T: ITableEntity, new();

        /// <summary>
        ///     Adds multiple new record to the table storage synchronously.
        /// </summary>
        /// <param name="record">The list of <see cref="ITableEntity" /> record.</param>
        void AddRecords<T>(IEnumerable<T> record) where T: ITableEntity, new();

        /// <summary>
        ///     Adds multiple new records to the table storage asynchronously.
        /// </summary>
        /// <param name="record">The list of <see cref="ITableEntity" /> records.</param>
        Task AddRecordsAsync<T>(IEnumerable<T> record, string tableName = null) where T: ITableEntity, new();

        /// <summary>
        ///     Adds or replaces multiple records to the table storage asynchronously.
        /// </summary>
        /// <param name="record">The list of <see cref="ITableEntity" /> records.</param>
        Task AddOrReplaceRecordsAsync<T>(IEnumerable<T> record, string tableName = null) where T : ITableEntity, new();

        /// <summary>
        ///     Removes a record from the table storage synchronously.
        /// </summary>
        /// <param name="record">The <see cref="ITableEntity" /> record.</param>
        void RemoveRecord<T>(T record) where T : ITableEntity, new();

        /// <summary>
        ///     Removes a record from the table storage asynchronously.
        /// </summary>
        /// <param name="record">The <see cref="ITableEntity" /> record.</param>
        Task RemoveRecordAsync<T>(T record) where T : ITableEntity, new();

        /// <summary>
        ///     Removes multiple records from the table storage synchronously.
        /// </summary>
        /// <param name="record">The list of <see cref="ITableEntity" /> records.</param>
        void RemoveRecords<T>(IEnumerable<T> record) where T : ITableEntity, new();

        /// <summary>
        ///     Removes multiple records from the table storage asynchronously.
        /// </summary>
        /// <param name="record">The list of <see cref="ITableEntity" /> records.</param>
        Task RemoveRecordsAsync<T>(IEnumerable<T> record) where T : ITableEntity, new();

        /// <summary>
        ///     Utility method that deletes a specific row in a given table asynchronously.
        /// </summary>
        Task DeleteRowAsync(string partitionKey, string rowKey);

        /// <summary>
        ///     Utility method that deletes all the rows in a given table synchronously.
        /// </summary>
        void DeleteAllRows();

        /// <summary>
        ///     Utility method that deletes all the rows in a given table asynchronously.
        /// </summary>
        Task DeleteAllRowsAsync();

        /// <summary>
        ///     Utility method that deletes the table asynchronously if it exists.
        /// </summary>
        Task DeleteIfExistsAsync();

        /// <summary>
        ///     Can be used to verify that table delete operation has completed. Empty table will be created in the process.
        /// </summary>
        Task EnsureTableDataDeletedAsync();

        /// <summary>
        ///     Method used to retrieve all the results of a given ODATA query in a single call.
        /// </summary>
        /// <typeparam name="T">The type of entity to be retrieved from table storage.</typeparam>
        /// <param name="filter">The ODATA query filter to be used to select the data to retrieve.</param>
        /// <returns>
        ///     An <see cref="IAsyncEnumerable{T}" /> of <see cref="T" />.
        /// </returns>
        IAsyncEnumerable<T> GetAllDataAsync<T>(string filter, IList<string> selectColumns = null) where T : ITableEntity, new();

        /// <summary>
        ///     Query execution method that provided pass-through parameter for <see cref="TableContinuationToken" /> so that
        ///     a client can control its own data ingestion.
        /// </summary>
        /// <typeparam name="T">The type of entity to be retrieved from table storage.</typeparam>
        /// <param name="filter">The ODATA query filter to be used to select the data to retrieve.</param>
        /// <param name="continuationToken">
        ///     The <see cref="TableContinuationToken" /> to be used for continuation queries, null if
        ///     it is the first query.
        /// </param>
        /// <returns>
        ///     An <see cref="IAsyncEnumerable{T}" /> of <see cref="QueryResult{T}" /> which contains
        ///     <see cref="QueryResult{T}.Entity" />
        ///     with a single record and <see cref="QueryResult{T}.ContinuationToken" /> with the continuation token for subsequent
        ///     queries.
        /// </returns>
        IAsyncEnumerable<QueryResult<T>> GetSegmentedDataAsync<T>(ICloudTableWrapper cloudTableReference, string filter,
            TableContinuationToken continuationToken = null, IList<string> selectColumns = null) where T : ITableEntity, new();

        Task<AllQueryResult<T>> GetAllSegmentedDataAsync<T>(ICloudTableWrapper cloudTableReference, string filter,
            TableContinuationToken continuationToken = null, IList<string> selectColumns = null, int? numberOfRecordsToTake = null) where T : ITableEntity, new();

        Task<AllQueryResult<T>> GetAllDataAsync<T>(
            ICloudTableWrapper cloudTableReference, 
            string filter,
            TableContinuationToken continuationToken = null, 
            TableQueryOptions queryOptions = null) where T : ITableEntity, new();

        /// <summary>
        /// Creates a range filter for a given property, that restricts values to only those that alphabetically fit between two strings.
        /// The filter performs an alphabetical comparison, (value >= Start) and (values < End).
        /// </summary>
        /// <param name="property">Property to apply the filter to.</param>
        /// <param name="start">Filter comparison start.</param>
        /// <param name="end">Filter comparison end.</param>
        /// 
        /// <example>
        /// Search for a partition key that starts with AA:
        /// GetRangeFIlter("PartitionKey", "AA", "AA")
        /// </example>
        /// 
        /// <example>
        /// Search for a partition key that can start with AA, AB, AC, AAB, CA, CBA up to CC, including CCA, CCC, but not CD or CDA:
        /// GetRangeFIlter("PartitionKey", "AA", "CC")
        /// </example>
        /// 
        /// <returns></returns>
        string GetRangeFilter(string property, string start, string end);
    }
}