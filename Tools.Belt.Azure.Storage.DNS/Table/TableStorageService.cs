using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Polly;
using Tools.Belt.Azure.Storage.DNS.Common;
using Tools.Belt.Azure.Storage.DNS.Extensions;
using Tools.Belt.Azure.Storage.DNS.Table.Structures.Query;
using Tools.Belt.Common.Extensions;
using Tools.Belt.Common.Logging;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    public class TableStorageService : ITableStorageService
    {
        public const int QueryTimeWarningMs = 60000;
        public const int MaxQueryUrlLength = 32684;
        private const double MaxFilterLength = MaxQueryUrlLength * 0.75;
        protected readonly ILogger Logger;
        protected ICloudStorageAccountWrapper CloudStorageAccountWrapper;
        protected const string PartitionKeyFieldName = "PartitionKey";
        protected const string RowKeyFieldName = "RowKey";

        public TableStorageService(ILogger logger, ITableStorageConfiguration configurationService):this(logger, configurationService, new CloudStorageAccountWrapper(CloudStorageAccount.Parse(configurationService.StorageAccountConnectionString)))
        {
        }


        public TableStorageService(ILogger logger, ITableStorageConfiguration configurationService, ICloudStorageAccountWrapper cloudStorageAccount)
        {
            ConfigurationService = configurationService;
            Logger = logger;
            CloudStorageAccountWrapper = cloudStorageAccount;
            StorageClient = CloudStorageAccountWrapper.CreateCloudTableClient();
        }

        public ITableStorageConfiguration ConfigurationService { get; }

        /// <inheritdoc />
        public ICloudTableClientWrapper StorageClient { get; protected set; }

        /// <inheritdoc />
        public bool Exists<T>(T record) where T : ITableEntity, new()
        {
            return ExistsAsync(record).ReturnAsyncCallSynchronously(CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync<T>(T record) where T : ITableEntity, new()
        {
            ICloudTableWrapper cloudTableReference = await GetCloudTableReferenceAsync(ConfigurationService.TableName,
                ConfigurationService.CreateIfNotExist);
            TableResult result = await cloudTableReference.ExecuteAsync(TableOperation.Retrieve(record.PartitionKey,
                record.RowKey, new List<string>(new[] { "RowKey" })));
            
            return result.HttpStatusCode.IsSuccessfulStatusCode();
        }

        /// <inheritdoc />
        public bool Exists<T>(IEnumerable<T> records) where T : ITableEntity, new()
        {
            return ExistsAsync(records).ReturnAsyncCallSynchronously(CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync<T>(IEnumerable<T> records) where T : ITableEntity, new()
        {
            IEnumerable<T> tableEntities = records.ToList();
            IList<T> result = await GetExistingAsync(tableEntities, new List<string>(new[] { "RowKey" }));
            return result.Count == tableEntities.Count();
        }

        public async Task<IList<T>> GetExistingAsync<T>(IEnumerable<T> records, List<string> selectColumns = null, string tableName = null) where T : ITableEntity, new()
        {
            IList<string> queries = GetRowkeyQueries(records);
            var taskList = new ConcurrentBag<Task<List<T>>>();
            ICloudTableWrapper cloudTableReference = await GetCloudTableReferenceAsync(tableName ?? ConfigurationService.TableName,
                ConfigurationService.CreateIfNotExist);

            var resultList = await queries
                .AsParallel()
                .ForAllAsync(query => 
                GetAllDataAsync<T>(query, cloudTableReference, selectColumns).ToListAsync(), 
                ConfigurationService.TableStorageMaxParallelRetrieve);

            return resultList.SelectMany(records => records).Where(record => record != null).ToList();
        }

        /// <inheritdoc />
        public void AddRecord<T>(T record) where T : ITableEntity, new()
        {
            AddRecordAsync(record).RunSynchronously();
        }

        /// <inheritdoc />
        public async Task AddRecordAsync<T>(T record) where T : ITableEntity, new()
        {

            await ModifyRecordsAsync(new[] { record }, TableOperation.Insert).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task AddOrReplaceRecordAsync<T>(T record) where T : ITableEntity, new()
        {
            await ModifyRecordsAsync(new[] { record }, TableOperation.InsertOrReplace).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void AddRecords<T>(IEnumerable<T> records) where T : ITableEntity, new()
        {           
            AddRecordsAsync(records).RunSynchronously();
        }

        /// <inheritdoc />
        public void RemoveRecord<T>(T record) where T : ITableEntity, new()
        {
            RemoveRecordAsync(record).RunSynchronously();
        }

        /// <inheritdoc />
        public async Task RemoveRecordAsync<T>(T record) where T : ITableEntity, new()
        {
            await RemoveRecordsAsync(new[] { record });
        }

        /// <inheritdoc />
        public void RemoveRecords<T>(IEnumerable<T> records) where T : ITableEntity, new()
        {
            RemoveRecordsAsync(records).RunSynchronously();
        }

        /// <inheritdoc />
        public void DeleteAllRows()
        {
            DeleteAllRowsAsync().RunSynchronously();
        }

        /// <inheritdoc />
        public async Task DeleteAllRowsAsync()
        {
            ICloudTableWrapper cloudTableReference = await GetCloudTableReferenceAsync(ConfigurationService.TableName,
                ConfigurationService.CreateIfNotExist);
            int batchSizeThreshold = ConfigurationService.TableStorageBatchSizeThreshold;
            TableQuery query = new TableQuery
            {
                SelectColumns = new List<string>
                {
                    nameof(TableEntity.PartitionKey),
                    nameof(TableEntity.RowKey)
                }
            };

            TableContinuationToken token = null;
            IDictionary<string, IList<TableBatchOperation>> batches = new Dictionary<string, IList<TableBatchOperation>>();
            
            do
            {
                TableQuerySegmentWrapper queryResult = await cloudTableReference.ExecuteQuerySegmentedAsync(query, token);
                foreach (DynamicTableEntity entity in queryResult.Results)
                {
                    if (!batches.ContainsKey(entity.PartitionKey))
                        batches.Add(entity.PartitionKey, new List<TableBatchOperation>());

                    if (batches[entity.PartitionKey].Count == 0 ||
                        batches[entity.PartitionKey].Last().Count == batchSizeThreshold)
                        batches[entity.PartitionKey].Add(new TableBatchOperation());

                    var batch = batches[entity.PartitionKey].Last();

                    batch.Delete(entity);
                }

                token = queryResult.ContinuationToken;
            } while (token != null);

            List<TableResult> results = new List<TableResult>();
            foreach (IList<TableBatchOperation> partitionBatches in batches.Values)
                foreach (TableBatchOperation batch in partitionBatches)
                    results.AddRange(await cloudTableReference.ExecuteBatchAsync(batch));
            
            results.EnsureNoErrors();
        }

        /// <inheritdoc />
        public async Task DeleteRowAsync(string partitionKey, string rowKey)
        {
            ICloudTableWrapper cloudTableReference = await GetCloudTableReferenceAsync(ConfigurationService.TableName,
                ConfigurationService.CreateIfNotExist);

            TableEntity entityToDelete = new TableEntity(partitionKey, rowKey) { ETag = "*" };
            TableOperation op = TableOperation.Delete(entityToDelete);
            TableResult deleteResult = await cloudTableReference.ExecuteAsync(op);

            deleteResult.EnsureNoErrors();
        }

        protected ICloudTableWrapper GetCloudTableReference(string tableName, bool createIfNotExist)
        {
            return GetCloudTableReferenceAsync(tableName, createIfNotExist).ReturnAsyncCallSynchronously();
        }

        protected async Task<ICloudTableWrapper> GetCloudTableReferenceAsync(string tableName, bool createIfNotExist)
        {
            ICloudTableWrapper cloudTableReference = StorageClient.GetTableReference(tableName);

            if (!createIfNotExist) return cloudTableReference;

            Logger.LogDebug($"Creating table {tableName} if it doesn't exist.");
            await cloudTableReference.CreateIfNotExistsAsync();

            return cloudTableReference;
        }

        public async Task CreateTableIfNotExistsAsync()
        {
            Console.WriteLine("In Create table if not exists");
            Console.WriteLine($"Connection string is: {ConfigurationService.StorageAccountConnectionString}");
            Console.WriteLine($"Configuration table name is: {ConfigurationService.TableName}");
            ICloudTableWrapper cloudTableReference = StorageClient.GetTableReference(ConfigurationService.TableName);
            
            await cloudTableReference.CreateIfNotExistsAsync();
        }

        public async Task EnsureTableDataDeletedAsync()
        {
            bool deleted = false;

            while (!deleted)
            { 
                try
                {
                    await CreateTableIfNotExistsAsync();
                    deleted = true;
                }
                catch(Exception ex) when (ex.Message == "Conflict")
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public async IAsyncEnumerable<T> GetAllDataAsync<T>(string filter, IList<string> selectColumns = null) where T : ITableEntity, new()
        {
            ICloudTableWrapper cloudTableReference = await GetCloudTableReference();

            TableContinuationToken continuationToken = null;
            do
            {
                await foreach (var entity in GetSegmentedDataAsync<T>(cloudTableReference, filter, continuationToken,
                    selectColumns))
                {
                    if (entity != null)
                    {
                        continuationToken = entity.ContinuationToken;
                        yield return entity.Entity;
                    }
                }
            } while (continuationToken != null);
        }

        public async IAsyncEnumerable<T> GetAllDataAsync<T>(string filter, ICloudTableWrapper cloudTableReference, IList<string> selectColumns = null) where T : ITableEntity, new()
        {
            TableContinuationToken continuationToken = null;
            do
            {
                await foreach (var entity in GetSegmentedDataAsync<T>(cloudTableReference, filter, continuationToken, selectColumns))
                {
                    if (entity != null)
                    {
                        continuationToken = entity.ContinuationToken;
                        yield return entity.Entity;
                    }
                }
            } while (continuationToken != null);
        }
        
        /// <inheritdoc />
        public string GetRangeFilter(string property, string start, string end)
        {
            int length = end.Length -1;
            int lastChar = end[length];
            char nextChar = (char)(lastChar + 1);
            end = end.Substring(0, length) + nextChar;

            string filterGt = TableQuery.GenerateFilterCondition(property, QueryComparisons.GreaterThanOrEqual, start);
            string filterLt = TableQuery.GenerateFilterCondition(property, QueryComparisons.LessThan, end);
            string filter = TableQuery.CombineFilters(filterGt, "and", filterLt);

            return filter;
        }

        public async Task<ICloudTableWrapper> GetCloudTableReference()
        {
            ICloudTableWrapper cloudTableReference = await GetCloudTableReferenceAsync(ConfigurationService.TableName,
                ConfigurationService.CreateIfNotExist);

            return cloudTableReference;
        }

        public async IAsyncEnumerable<QueryResult<T>> GetSegmentedDataAsync<T>(ICloudTableWrapper cloudTableReference, string filter,
            TableContinuationToken continuationToken = null, IList<string> selectColumns = null) where T : ITableEntity, new()
        {
            TableQuery<T> query = new TableQuery<T> { FilterString = filter };
            ITableQuerySegmentWrapper<T> segment = null;
            bool notFound = false;

            if (selectColumns != null)
            {
                query.SelectColumns = selectColumns;
            }

            TimeSpan queryTime;
            try
            {
                using (var tl = new TimeoutLog(TimeSpan.FromMilliseconds(QueryTimeWarningMs), Logger, $"Query '{filter}' lasted over {QueryTimeWarningMs}."))
                {
                    segment = await cloudTableReference.ExecuteQuerySegmentedAsync(query, continuationToken);
                    queryTime = tl.Elapsed;
                }
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode != (int)System.Net.HttpStatusCode.NotFound)
                {
                    throw ex;
                }

                notFound = true;
            }

            if (notFound)
            {
                yield return null;
            }
            else
            {
                foreach (T entity in segment.Results)
                {
                    yield return new QueryResult<T>(entity, segment.ContinuationToken, filter, queryTime);
                }
            }
        }

        public async Task<AllQueryResult<T>> GetAllSegmentedDataAsync<T>(ICloudTableWrapper cloudTableReference, string filter,
            TableContinuationToken continuationToken = null, IList<string> selectColumns = null, int? numberOfRecordsToTake = null) where T : ITableEntity, new()
        {
            TableQuery<T> query = new TableQuery<T> { FilterString = filter };

            if (selectColumns != null)
            {
                query.SelectColumns = selectColumns;
            }

            if (numberOfRecordsToTake != null) query.Take(numberOfRecordsToTake);

            using (var tl = new TimeoutLog(TimeSpan.FromMilliseconds(QueryTimeWarningMs), Logger, $"Query '{filter}' lasted over {QueryTimeWarningMs}."))
            {
                ITableQuerySegmentWrapper<T> segment =
                    await cloudTableReference.ExecuteQuerySegmentedAsync(query, continuationToken);

                return new AllQueryResult<T>(segment.Results, segment.ContinuationToken, filter, true, tl.Elapsed);
            }
        }

        public async Task<AllQueryResult<T>> GetAllDataAsync<T>(
            ICloudTableWrapper cloudTableReference, 
            string filter,
            TableContinuationToken continuationToken = null, 
            TableQueryOptions queryOptions = null) where T : ITableEntity, new()
        {
            if (queryOptions == null) queryOptions = new TableQueryOptions();

            TableQuery<T> query = new TableQuery<T> { FilterString = filter };
            List<T> records = new List<T>();
            var queryTimes = new List<TimeSpan>();
            int iterationCount = 0;
            bool started = continuationToken != null ? true : false;

            if (!MaxQueryTimeExceeded(queryOptions.QueryTimer, queryOptions.MaxQueryRunningSeconds))
            { 
                if (queryOptions.SelectColumns != null) query.SelectColumns = queryOptions.SelectColumns;
                if (queryOptions.NumberOfRecordsToTake != null) query.Take(queryOptions.NumberOfRecordsToTake);

                do
                {
                    iterationCount++;
                
                    ITableQuerySegmentWrapper<T> segment;
                    using (var tl = new TimeoutLog(TimeSpan.FromMilliseconds(QueryTimeWarningMs), Logger, $"Query '{filter}' lasted over {QueryTimeWarningMs}."))
                    {
                        segment = await cloudTableReference.ExecuteQuerySegmentedAsync(query, continuationToken);
                        Logger.LogDebug($"Iteration took: {tl.Elapsed.Milliseconds}");
                        queryTimes.Add(tl.Elapsed);
                    }

                    records.AddRange(segment.Results);

                    if (queryOptions.NumberOfRecordsToTake != null && queryOptions.NumberOfRecordsToTake <= records.Count)
                    {
                        continuationToken = null;
                    }
                    else
                    {
                        continuationToken = segment.ContinuationToken;
                    }

                    started = true;
                }
                while (continuationToken != null && iterationCount < queryOptions.MaxIterations && 
                    !MaxQueryTimeExceeded(queryOptions.QueryTimer, queryOptions.MaxQueryRunningSeconds));
            }

            if (queryOptions.NumberOfRecordsToTake != null) records = records.Take(queryOptions.NumberOfRecordsToTake.Value).ToList();

            return new AllQueryResult<T>(records, continuationToken, filter, started, queryTimes);
        }

        /// <inheritdoc />
        public async Task AddOrReplaceRecordsAsync<T>(IEnumerable<T> records, string tableName = null) where T : ITableEntity, new()
        {
            await ModifyRecordsAsync(records, TableOperation.InsertOrReplace, tableName).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AddRecordsAsync<T>(IEnumerable<T> records, string tableName = null) where T : ITableEntity, new()
        {
            await ModifyRecordsAsync(records, TableOperation.Insert, tableName).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task RemoveRecordsAsync<T>(IEnumerable<T> records) where T : ITableEntity, new()
        {
            await ModifyRecordsAsync(records, TableOperation.Delete).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ModifyRecordsAsync<T>(IEnumerable<T> records, Func<ITableEntity, TableOperation> tableOperation, string tableName = null) where T : ITableEntity, new()
        {
            int retryLimit = ConfigurationService.SystemConfiguration.PollyRetryLimit;
            TimeSpan retryDelay = TimeSpan.FromMilliseconds(ConfigurationService.SystemConfiguration.PollyRetryWaitMs);
            int batchSizeThreshold = ConfigurationService.TableStorageBatchSizeThreshold;
            var enumeratedRecords = records.ToList();

            if (enumeratedRecords.Any() == false) return;

            Logger.LogTrace(
                $"Obtaining Table reference for table: {tableName ?? ConfigurationService.TableName} on storage: {CloudStorageAccountWrapper.TableStorageUri.PrimaryUri} with conn string: {CloudStorageAccountWrapper.GetSharedAccessSignature(new SharedAccessAccountPolicy())}");
            ICloudTableWrapper cloudTableReference = await GetCloudTableReferenceAsync(tableName ?? ConfigurationService.TableName,
                ConfigurationService.CreateIfNotExist);
            Logger.LogDebug($"Creating batched operation for {enumeratedRecords.Count()}");

            List<TableBatchOperation> batches = new List<TableBatchOperation>();
            TableBatchOperation operation = new TableBatchOperation();
            foreach (string partitionKey in enumeratedRecords.Select(item => item.PartitionKey).Distinct())
            {
                foreach (T entity in enumeratedRecords.Where(entity => entity.PartitionKey == partitionKey))
                {
                    if (operation.Count >= batchSizeThreshold)
                        AddBatchToList(ref batches, ref operation);

                    TableOperation to = tableOperation.Invoke(entity);
                    if (operation.Contains(to) == false)
                        operation.Add(to);
                }

                AddBatchToList(ref batches, ref operation);
            }

            List<TableResult> results = new List<TableResult>();
            foreach (TableBatchOperation batchOperation in batches)
                await Policy.Handle<Exception>(exception => RetryCondition(exception))
                    .WaitAndRetryAsync(retryLimit, i => retryDelay)
                    .ExecuteAsync(async () =>
                    {
                        BeforeExecuteOperation(batchOperation, enumeratedRecords);
                        results.AddRange(await cloudTableReference.ExecuteBatchAsync(batchOperation));
                        AfterExecuteOperation(batchOperation, enumeratedRecords);
                    });

            results.EnsureNoErrors();
        }

        protected virtual void BeforeExecuteOperation<T>(TableBatchOperation operation,
            List<T> enumeratedRecords) where T : ITableEntity, new()
        {
        }

        protected virtual void AfterExecuteOperation<T>(TableBatchOperation operation, List<T> enumeratedRecords)
            where T : ITableEntity, new()
        {
        }

        public void AddBatchToList(ref List<TableBatchOperation> source, ref TableBatchOperation operation)
        {
            if (operation.Count == 0) return;
            source.Add(operation);
            operation = new TableBatchOperation();
        }

        public async Task DeleteIfExistsAsync()
        {
            ICloudTableWrapper cloudTableReference = await GetCloudTableReferenceAsync(ConfigurationService.TableName, false);

            if (cloudTableReference != null) await cloudTableReference.DeleteIfExistsAsync();
        }

        private bool RetryCondition(Exception e)
        {
            if ((e is StorageException) && e.Message.Contains("The specified entity already exists")) return false;

            return true;
        }

        private IList<string> GetRowkeyQueries<T>(IEnumerable<T> records) where T : ITableEntity, new()
        {
            IList<string> queries = new List<string>();

            foreach (var record in records)
            {
                string partitionKeyFilter = TableQuery.GenerateFilterCondition(PartitionKeyFieldName, QueryComparisons.Equal, record.PartitionKey);
                string rowkeyFilter = TableQuery.GenerateFilterCondition(RowKeyFieldName, QueryComparisons.Equal, record.RowKey);
                queries.Add(TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, rowkeyFilter));
            }

            return queries;
        }

        private bool MaxQueryTimeExceeded(Stopwatch queryTimer, int? maxQueryRunningSeconds)
        {
            if (queryTimer != null && maxQueryRunningSeconds != null &&
                queryTimer.Elapsed.TotalSeconds >= maxQueryRunningSeconds) return true;

            return false;
        }
    }
}