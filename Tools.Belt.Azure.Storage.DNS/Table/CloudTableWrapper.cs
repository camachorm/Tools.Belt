using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    public class CloudTableWrapper : ICloudTableWrapper
    {
        private readonly CloudTable _cloudTable;

        public CloudTableWrapper(CloudTable cloudTable)
        {
            this._cloudTable = cloudTable;
        }

        public CloudTableWrapper()
        {
        }

        public CloudTableClient ServiceClient { get; }
        public string Name { get; }
        public Uri Uri { get; }
        public StorageUri StorageUri { get; }
        public string GetSharedAccessSignature(SharedAccessTablePolicy policy)
        {
            if (_cloudTable != null) return _cloudTable.GetSharedAccessSignature(policy);
            throw new NotImplementedException();
        }

        public string GetSharedAccessSignature(SharedAccessTablePolicy policy, string accessPolicyIdentifier)
        {
            if (_cloudTable != null) return _cloudTable.GetSharedAccessSignature(policy, accessPolicyIdentifier);
            throw new NotImplementedException();
        }

        public string GetSharedAccessSignature(SharedAccessTablePolicy policy, string accessPolicyIdentifier, string startPartitionKey,
            string startRowKey, string endPartitionKey, string endRowKey)
        {
            return _cloudTable != null
                ? _cloudTable.GetSharedAccessSignature(policy, accessPolicyIdentifier, startPartitionKey,
                    startRowKey, endPartitionKey, endRowKey)
                : "";
        }

        public string GetSharedAccessSignature(SharedAccessTablePolicy policy, string accessPolicyIdentifier, string startPartitionKey,
            string startRowKey, string endPartitionKey, string endRowKey, SharedAccessProtocol? protocols,
            IPAddressOrRange ipAddressOrRange)
        {
            if (_cloudTable != null)
                return _cloudTable.GetSharedAccessSignature(policy, accessPolicyIdentifier, startPartitionKey,
                    startRowKey, endPartitionKey, endRowKey, protocols, ipAddressOrRange);
            throw new NotImplementedException();
        }

        public async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteAsync(operation);
            throw new NotImplementedException();
        }

        public async Task<TableResult> ExecuteAsync(TableOperation operation, TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteAsync(operation, requestOptions, operationContext);
            return new TableResult();
        }

        public async Task<TableResult> ExecuteAsync(TableOperation operation, TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteAsync(operation, requestOptions, operationContext, cancellationToken);
            return new TableResult();
        }

        public async Task<IList<TableResult>> ExecuteBatchAsync(TableBatchOperation batch)
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteBatchAsync(batch);
            return new List<TableResult>();
        }

        public async Task<IList<TableResult>> ExecuteBatchAsync(TableBatchOperation batch, TableRequestOptions requestOptions,
            OperationContext operationContext)
        {
            if (_cloudTable != null)
                return await _cloudTable.ExecuteBatchAsync(batch, requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<IList<TableResult>> ExecuteBatchAsync(TableBatchOperation batch, TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null)
                return await _cloudTable.ExecuteBatchAsync(batch, requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegmentWrapper> ExecuteQuerySegmentedAsync(TableQuery query, TableContinuationToken token)
        {
            if (_cloudTable != null) return new TableQuerySegmentWrapper(await _cloudTable.ExecuteQuerySegmentedAsync(query, token));
            return new TableQuerySegmentWrapper(new List<DynamicTableEntity>(new[] { new DynamicTableEntity() }));
        }

        public async Task<TableQuerySegment> ExecuteQuerySegmentedAsync(TableQuery query, TableContinuationToken token, TableRequestOptions requestOptions,
            OperationContext operationContext)
        {
            return _cloudTable != null
                ? await _cloudTable.ExecuteQuerySegmentedAsync(query, token, requestOptions, operationContext)
                : null;
        }

        public async Task<TableQuerySegment> ExecuteQuerySegmentedAsync(TableQuery query, TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext, CancellationToken cancellationToken)
        {
            return _cloudTable != null
                ? await _cloudTable.ExecuteQuerySegmentedAsync(query, token, requestOptions, operationContext,
                    cancellationToken)
                : null;
        }

        public async Task CreateAsync()
        {
            if (_cloudTable != null) await _cloudTable.CreateAsync();
            throw new NotImplementedException();
        }

        public async Task CreateAsync(TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null) await _cloudTable.CreateAsync(requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task CreateAsync(TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null) await _cloudTable.CreateAsync(requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task<bool> CreateIfNotExistsAsync()
        {
            if (_cloudTable != null) return await _cloudTable.CreateIfNotExistsAsync();
            throw new NotImplementedException();
        }

        public async Task<bool> CreateIfNotExistsAsync(TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null) return await _cloudTable.CreateIfNotExistsAsync(requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<bool> CreateIfNotExistsAsync(TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null) return await _cloudTable.CreateIfNotExistsAsync(requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task DeleteAsync()
        {
            if (_cloudTable != null)
            {
                await _cloudTable?.DeleteAsync();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task DeleteAsync(TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null)
            {
                await _cloudTable.DeleteAsync(requestOptions, operationContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task DeleteAsync(TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null) await _cloudTable.DeleteAsync(requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteIfExistsAsync()
        {
            if (_cloudTable != null) return await _cloudTable.DeleteIfExistsAsync();
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteIfExistsAsync(TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null) return await _cloudTable.DeleteIfExistsAsync(requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteIfExistsAsync(TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null) return await _cloudTable.DeleteIfExistsAsync(requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync()
        {
            if (_cloudTable != null) return await _cloudTable.ExistsAsync();
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null) return await _cloudTable.ExistsAsync(requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null) return await _cloudTable.ExistsAsync(requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task SetPermissionsAsync(TablePermissions permissions)
        {
            if (_cloudTable != null) await _cloudTable.SetPermissionsAsync(permissions);
            throw new NotImplementedException();
        }

        public async Task SetPermissionsAsync(TablePermissions permissions, TableRequestOptions requestOptions,
            OperationContext operationContext)
        {
            if (_cloudTable != null)
            {
                await _cloudTable.SetPermissionsAsync(permissions, requestOptions, operationContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task SetPermissionsAsync(TablePermissions permissions, TableRequestOptions requestOptions,
            OperationContext operationContext, CancellationToken cancellationToken)
        {
            if (_cloudTable != null)
            {
                await _cloudTable.SetPermissionsAsync(permissions, requestOptions, operationContext, cancellationToken);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task<TablePermissions> GetPermissionsAsync()
        {
            if (_cloudTable != null) return await _cloudTable.GetPermissionsAsync();
            throw new NotImplementedException();
        }

        public async Task<TablePermissions> GetPermissionsAsync(TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null) return await _cloudTable.GetPermissionsAsync(requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<TablePermissions> GetPermissionsAsync(TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_cloudTable != null) return await _cloudTable.GetPermissionsAsync(requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public async Task<ITableQuerySegmentWrapper<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken token) 
            where T : ITableEntity, new()
        {
            //Method not testable due to class type constraint
            if (_cloudTable != null) return new TableQuerySegmentWrapper<T>(await _cloudTable.ExecuteQuerySegmentedAsync(query, token));
            return new TableQuerySegmentWrapper<T>();
        }

        public async Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken token, TableRequestOptions requestOptions,
            OperationContext operationContext) where T : ITableEntity, new()
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteQuerySegmentedAsync(query, token, requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(TableQuery<T> query, TableContinuationToken token, TableRequestOptions requestOptions,
            OperationContext operationContext, CancellationToken cancellationToken) where T : ITableEntity, new()
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteQuerySegmentedAsync(query, token, requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<T, TResult>(TableQuery<T> query, EntityResolver<TResult> resolver, TableContinuationToken token) where T : ITableEntity, new()
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteQuerySegmentedAsync(query, resolver, token);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<T, TResult>(TableQuery<T> query, EntityResolver<TResult> resolver, TableContinuationToken token,
            TableRequestOptions requestOptions, OperationContext operationContext) where T : ITableEntity, new()
        {
            if (_cloudTable != null)return await _cloudTable.ExecuteQuerySegmentedAsync(query, resolver, token, requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<T, TResult>(TableQuery<T> query, EntityResolver<TResult> resolver, TableContinuationToken token,
            TableRequestOptions requestOptions, OperationContext operationContext, CancellationToken cancellationToken) where T : ITableEntity, new()
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteQuerySegmentedAsync(query, resolver, token, requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(TableQuery query, EntityResolver<TResult> resolver, TableContinuationToken token)
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteQuerySegmentedAsync(query, resolver, token);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(TableQuery query, EntityResolver<TResult> resolver, TableContinuationToken token,
            TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteQuerySegmentedAsync(query, resolver, token, requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public async Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(TableQuery query, EntityResolver<TResult> resolver, TableContinuationToken token,
            TableRequestOptions requestOptions, OperationContext operationContext, CancellationToken cancellationToken)
        {
            if (_cloudTable != null) return await _cloudTable.ExecuteQuerySegmentedAsync(query, resolver, token, requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }
    }
}