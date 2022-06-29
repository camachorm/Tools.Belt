using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    public interface ICloudTableWrapper
    {
        CloudTableClient ServiceClient { get; }
        string Name { get; }
        Uri Uri { get; }
        StorageUri StorageUri { get; }
        string GetSharedAccessSignature(SharedAccessTablePolicy policy);

        string GetSharedAccessSignature(
            SharedAccessTablePolicy policy,
            string accessPolicyIdentifier);

        string GetSharedAccessSignature(
            SharedAccessTablePolicy policy,
            string accessPolicyIdentifier,
            string startPartitionKey,
            string startRowKey,
            string endPartitionKey,
            string endRowKey);

        string GetSharedAccessSignature(
            SharedAccessTablePolicy policy,
            string accessPolicyIdentifier,
            string startPartitionKey,
            string startRowKey,
            string endPartitionKey,
            string endRowKey,
            SharedAccessProtocol? protocols,
            IPAddressOrRange ipAddressOrRange);

        string ToString();
        Task<TableResult> ExecuteAsync(TableOperation operation);

        Task<TableResult> ExecuteAsync(
            TableOperation operation,
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<TableResult> ExecuteAsync(
            TableOperation operation,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<IList<TableResult>> ExecuteBatchAsync(
            TableBatchOperation batch);

        Task<IList<TableResult>> ExecuteBatchAsync(
            TableBatchOperation batch,
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<IList<TableResult>> ExecuteBatchAsync(
            TableBatchOperation batch,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<TableQuerySegmentWrapper> ExecuteQuerySegmentedAsync(
            TableQuery query,
            TableContinuationToken token);

        Task<TableQuerySegment> ExecuteQuerySegmentedAsync(
            TableQuery query,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<TableQuerySegment> ExecuteQuerySegmentedAsync(
            TableQuery query,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task CreateAsync();

        Task CreateAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task CreateAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<bool> CreateIfNotExistsAsync();

        Task<bool> CreateIfNotExistsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<bool> CreateIfNotExistsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task DeleteAsync();

        Task DeleteAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task DeleteAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<bool> DeleteIfExistsAsync();

        Task<bool> DeleteIfExistsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<bool> DeleteIfExistsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<bool> ExistsAsync();

        Task<bool> ExistsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<bool> ExistsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task SetPermissionsAsync(TablePermissions permissions);

        Task SetPermissionsAsync(
            TablePermissions permissions,
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task SetPermissionsAsync(
            TablePermissions permissions,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<TablePermissions> GetPermissionsAsync();

        Task<TablePermissions> GetPermissionsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<TablePermissions> GetPermissionsAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<ITableQuerySegmentWrapper<T>> ExecuteQuerySegmentedAsync<T>(
            TableQuery<T> query,
            TableContinuationToken token)
            where T : ITableEntity, new();

        Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(
            TableQuery<T> query,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext)
            where T : ITableEntity, new();

        Task<TableQuerySegment<T>> ExecuteQuerySegmentedAsync<T>(
            TableQuery<T> query,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken)
            where T : ITableEntity, new();

        Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<T, TResult>(
            TableQuery<T> query,
            EntityResolver<TResult> resolver,
            TableContinuationToken token)
            where T : ITableEntity, new();

        Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<T, TResult>(
            TableQuery<T> query,
            EntityResolver<TResult> resolver,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext)
            where T : ITableEntity, new();

        Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<T, TResult>(
            TableQuery<T> query,
            EntityResolver<TResult> resolver,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken)
            where T : ITableEntity, new();

        Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
            TableQuery query,
            EntityResolver<TResult> resolver,
            TableContinuationToken token);

        Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
            TableQuery query,
            EntityResolver<TResult> resolver,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<TableQuerySegment<TResult>> ExecuteQuerySegmentedAsync<TResult>(
            TableQuery query,
            EntityResolver<TResult> resolver,
            TableContinuationToken token,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);
    }
}