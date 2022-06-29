using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tools.Belt.Azure.Storage.DNS.Table
{
    class CloudTableClientWrapper : ICloudTableClientWrapper
    {

        private readonly CloudTableClient _client;

        public CloudTableClientWrapper(Uri baseUri, StorageCredentials credentials) : this(
            new CloudTableClient(baseUri, credentials))
        { }

        public CloudTableClientWrapper(StorageUri storageUri, StorageCredentials credentials) : this(new CloudTableClient(storageUri, credentials))
        { }

        public CloudTableClientWrapper(CloudTableClient createCloudTableClient)
        {
            _client = createCloudTableClient;
        }

        public CloudTableClientWrapper()
        {

        }

        public IBufferManager BufferManager { get; set; }
        public StorageCredentials Credentials => _client?.Credentials;
        public Uri BaseUri => _client?.BaseUri;
        public StorageUri StorageUri => _client?.StorageUri;
        public TableRequestOptions DefaultRequestOptions
        {
            get => _client?.DefaultRequestOptions;
            set => _client.DefaultRequestOptions = value;
        }
        public AuthenticationScheme AuthenticationScheme
        {
            get => _client.AuthenticationScheme;
            set => _client.AuthenticationScheme = value;
        }

        public ICloudTableWrapper GetTableReference(string tableName)
        {
            if (_client != null) return new CloudTableWrapper(_client.GetTableReference(tableName));
            return  new CloudTableWrapper();
        }

        public Task<TableResultSegment> ListTablesSegmentedAsync(TableContinuationToken currentToken)
        {
            if (_client != null) return _client.ListTablesSegmentedAsync(currentToken);
            throw new NotImplementedException();
        }

        public Task<TableResultSegment> ListTablesSegmentedAsync(string prefix, TableContinuationToken currentToken)
        {
            if (_client != null) return _client.ListTablesSegmentedAsync(prefix,currentToken);
            throw new NotImplementedException();
        }

        public Task<TableResultSegment> ListTablesSegmentedAsync(string prefix, int? maxResults, TableContinuationToken currentToken,
            TableRequestOptions requestOptions, OperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public Task<TableResultSegment> ListTablesSegmentedAsync(string prefix, int? maxResults, TableContinuationToken currentToken,
            TableRequestOptions requestOptions, OperationContext operationContext, CancellationToken cancellationToken)
        {
            if (_client != null)
                return _client.ListTablesSegmentedAsync(prefix, maxResults, currentToken, requestOptions,
                    operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public Task<ServiceProperties> GetServicePropertiesAsync()
        {
            if (_client != null) return _client.GetServicePropertiesAsync();
            throw new NotImplementedException();
        }

        public Task<ServiceProperties> GetServicePropertiesAsync(TableRequestOptions requestOptions, OperationContext operationContext)
        {
            if (_client != null) return _client.GetServicePropertiesAsync(requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public Task<ServiceProperties> GetServicePropertiesAsync(TableRequestOptions requestOptions, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_client != null)
                return _client.GetServicePropertiesAsync(requestOptions, operationContext, cancellationToken);
            throw new NotImplementedException();
        }

        public Task SetServicePropertiesAsync(ServiceProperties properties)
        {
            if (_client != null) return _client.SetServicePropertiesAsync(properties);
            throw new NotImplementedException();
        }

        public Task SetServicePropertiesAsync(ServiceProperties properties, TableRequestOptions requestOptions,
            OperationContext operationContext)
        {
            if (_client != null) return _client.SetServicePropertiesAsync(properties, requestOptions, operationContext);
            throw new NotImplementedException();
        }

        public Task SetServicePropertiesAsync(ServiceProperties properties, TableRequestOptions requestOptions,
            OperationContext operationContext, CancellationToken cancellationToken)
        {
            if (_client != null)
                return _client.SetServicePropertiesAsync(properties, requestOptions, operationContext,
                    cancellationToken);
            throw new NotImplementedException();
        }

        public Task<ServiceStats> GetServiceStatsAsync()
        {
            if (_client != null) return _client.GetServiceStatsAsync();
            throw new NotImplementedException();
        }

        public Task<ServiceStats> GetServiceStatsAsync(TableRequestOptions options, OperationContext operationContext)
        {
            if (_client != null) return _client.GetServiceStatsAsync(options, operationContext);
            throw new NotImplementedException();
        }

        public Task<ServiceStats> GetServiceStatsAsync(TableRequestOptions options, OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_client != null) return _client.GetServiceStatsAsync(options, operationContext, cancellationToken);
            throw new NotImplementedException();
        }
    }
}