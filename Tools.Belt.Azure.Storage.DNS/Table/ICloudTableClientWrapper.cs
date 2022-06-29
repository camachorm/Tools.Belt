using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    public interface ICloudTableClientWrapper
    {
        IBufferManager BufferManager { get; set; }
        StorageCredentials Credentials { get; }
        Uri BaseUri { get; }
        StorageUri StorageUri { get; }
        TableRequestOptions DefaultRequestOptions { get; set; }
        AuthenticationScheme AuthenticationScheme { get; set; }
        ICloudTableWrapper GetTableReference(string tableName);

        Task<TableResultSegment> ListTablesSegmentedAsync(
            TableContinuationToken currentToken);

        Task<TableResultSegment> ListTablesSegmentedAsync(
            string prefix,
            TableContinuationToken currentToken);

        Task<TableResultSegment> ListTablesSegmentedAsync(
            string prefix,
            int? maxResults,
            TableContinuationToken currentToken,
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<TableResultSegment> ListTablesSegmentedAsync(
            string prefix,
            int? maxResults,
            TableContinuationToken currentToken,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<ServiceProperties> GetServicePropertiesAsync();

        Task<ServiceProperties> GetServicePropertiesAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task<ServiceProperties> GetServicePropertiesAsync(
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task SetServicePropertiesAsync(ServiceProperties properties);

        Task SetServicePropertiesAsync(
            ServiceProperties properties,
            TableRequestOptions requestOptions,
            OperationContext operationContext);

        Task SetServicePropertiesAsync(
            ServiceProperties properties,
            TableRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<ServiceStats> GetServiceStatsAsync();

        Task<ServiceStats> GetServiceStatsAsync(
            TableRequestOptions options,
            OperationContext operationContext);

        Task<ServiceStats> GetServiceStatsAsync(
            TableRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken);
    }
}