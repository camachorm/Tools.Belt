// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using Microsoft.Extensions.Logging;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public class CloudBlobClientWrapper : ICloudBlobClientWrapper
    {
        private CloudBlobClient _client;
        private readonly ILogger _logger;

        public CloudBlobClientWrapper(Uri baseUri, ILogger logger) : this(new CloudBlobClient(baseUri), logger)
        {
            AccountName = baseUri.AbsoluteUri;
            BaseUri = baseUri;
            logger.LogDebug(
                $"Initialized CloudBlobClientWrapper from CloudBlobClientWrapper(Uri baseUri, ILogger logger) with uri: {AccountName}");
        }

        public CloudBlobClientWrapper(Uri baseUri, StorageCredentials credentials, ILogger logger) : this(
            new CloudBlobClient(baseUri, credentials), logger)
        {
            AccountName = baseUri.AbsoluteUri;
            BaseUri = baseUri;
            logger.LogDebug(
                $"Initialized CloudBlobClientWrapper from CloudBlobClientWrapper(Uri baseUri, StorageCredentials credentials, ILogger logger) with uri: {AccountName}");
        }

        public CloudBlobClientWrapper(StorageUri storageUri, StorageCredentials credentials, ILogger logger) : this(
            new CloudBlobClient(storageUri, credentials), logger)
        {
            AccountName = storageUri.PrimaryUri.AbsoluteUri;
            BaseUri = storageUri.PrimaryUri;
            StorageUri = storageUri;
            logger.LogDebug(
                $"Initialized CloudBlobClientWrapper from CloudBlobClientWrapper(StorageUri storageUri, StorageCredentials credentials, ILogger logger) with uri: {AccountName}");
        }

        public CloudBlobClientWrapper(CloudStorageAccount storageAccount, ILogger logger) : this(
            storageAccount.CreateCloudBlobClient(), logger)
        {
            AccountName = storageAccount.BlobEndpoint.AbsoluteUri;
            logger.LogDebug($"Initialized CloudBlobClientWrapper from CloudBlobClientWrapper(CloudStorageAccountWrapper storageUri, StorageCredentials credentials, ILogger logger) with uri: {AccountName}");
        }

        protected CloudBlobClientWrapper(CloudBlobClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public string AccountName { get; }

        public IBufferManager BufferManager { get; set; }

        public StorageCredentials Credentials { get; private set; }

        public Uri BaseUri { get; }

        public StorageUri StorageUri { get; private set; }

        public BlobRequestOptions DefaultRequestOptions { get; set; }

        [Obsolete("Use DefaultRequestOptions.RetryPolicy.")]
        public IRetryPolicy RetryPolicy { get; set; }

        public string DefaultDelimiter { get; set; }

        public AuthenticationScheme AuthenticationScheme { get; set; }

        public virtual CloudBlobContainer GetRootContainerReference()
        {
            return _client?.GetRootContainerReference();
        }

        public virtual CloudBlobContainer GetContainerReference(string containerName)
        {
            return _client?.GetContainerReference(containerName);
        }

        [DoesServiceRequest]
        public virtual async Task<ContainerResultSegment> ListContainersSegmentedAsync(
            BlobContinuationToken currentToken)
        {
            return _client != null ? await _client?.ListContainersSegmentedAsync(currentToken) : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ContainerResultSegment> ListContainersSegmentedAsync(
            string prefix,
            BlobContinuationToken currentToken)
        {
            return await _client.ListContainersSegmentedAsync(prefix, currentToken);
        }

        [DoesServiceRequest]
        public virtual async Task<ContainerResultSegment> ListContainersSegmentedAsync(
            string prefix,
            ContainerListingDetails detailsIncluded,
            int? maxResults,
            BlobContinuationToken currentToken,
            BlobRequestOptions options,
            OperationContext operationContext)
        {
            return _client != null
                ? await _client.ListContainersSegmentedAsync(
                    prefix,
                    detailsIncluded,
                    maxResults,
                    currentToken,
                    options,
                    operationContext)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ContainerResultSegment> ListContainersSegmentedAsync(
            string prefix,
            ContainerListingDetails detailsIncluded,
            int? maxResults,
            BlobContinuationToken currentToken,
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            return _client != null
                ? await _client.ListContainersSegmentedAsync(
                    prefix,
                    detailsIncluded,
                    maxResults,
                    currentToken,
                    options,
                    operationContext,
                    cancellationToken)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task<BlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            BlobContinuationToken currentToken)
        {
            return _client != null ? await _client.ListBlobsSegmentedAsync(prefix, currentToken) : null;
        }

        [DoesServiceRequest]
        public virtual async Task<BlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken,
            BlobRequestOptions options,
            OperationContext operationContext)
        {
            return _client != null
                ? await _client.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, maxResults,
                    currentToken, options, operationContext)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ICloudBlob> GetBlobReferenceFromServerAsync(Uri blobUri)
        {
            return _client != null ? await _client.GetBlobReferenceFromServerAsync(blobUri) : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            Uri blobUri,
            AccessCondition accessCondition,
            BlobRequestOptions options,
            OperationContext operationContext)
        {
            return _client != null
                ? await _client.GetBlobReferenceFromServerAsync(blobUri, accessCondition, options, operationContext)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            StorageUri blobUri,
            AccessCondition accessCondition,
            BlobRequestOptions options,
            OperationContext operationContext)
        {
            return _client != null
                ? await _client.GetBlobReferenceFromServerAsync(blobUri, accessCondition, options, operationContext)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            StorageUri blobUri,
            AccessCondition accessCondition,
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            return _client != null
                ? await _client.GetBlobReferenceFromServerAsync(blobUri, accessCondition, options, operationContext,
                    cancellationToken)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task<AccountProperties> GetAccountPropertiesAsync()
        {
            return _client != null ? await _client.GetAccountPropertiesAsync() : null;
        }

        [DoesServiceRequest]
        public virtual async Task<AccountProperties> GetAccountPropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext)
        {
            return _client != null ? await _client.GetAccountPropertiesAsync(options, operationContext) : null;
        }

        [DoesServiceRequest]
        public virtual async Task<AccountProperties> GetAccountPropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            return _client != null
                ? await _client.GetAccountPropertiesAsync(options, operationContext, cancellationToken)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ServiceProperties> GetServicePropertiesAsync()
        {
            return _client != null ? await _client.GetServicePropertiesAsync() : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ServiceProperties> GetServicePropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext)
        {
            return _client != null ? await _client.GetServicePropertiesAsync(options, operationContext) : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ServiceProperties> GetServicePropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            return _client != null
                ? await _client.GetServicePropertiesAsync(options, operationContext, cancellationToken)
                : null;
        }

        [DoesServiceRequest]
        public virtual async Task SetServicePropertiesAsync(ServiceProperties properties)
        {
            if (_client == null) return;
            await _client.SetServicePropertiesAsync(properties);
        }

        [DoesServiceRequest]
        public virtual async Task SetServicePropertiesAsync(
            ServiceProperties properties,
            BlobRequestOptions requestOptions,
            OperationContext operationContext)
        {
            if (_client == null) return;
            await _client.SetServicePropertiesAsync(properties, requestOptions, operationContext);
        }

        [DoesServiceRequest]
        public virtual async Task SetServicePropertiesAsync(
            ServiceProperties properties,
            BlobRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            if (_client == null) return;
            await _client.SetServicePropertiesAsync(properties, requestOptions, operationContext, cancellationToken);
        }

        [DoesServiceRequest]
        public virtual async Task<ServiceStats> GetServiceStatsAsync()
        {
            return _client != null ? await _client.GetServiceStatsAsync() : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ServiceStats> GetServiceStatsAsync(
            BlobRequestOptions options,
            OperationContext operationContext)
        {
            return _client != null ? await _client.GetServiceStatsAsync(options, operationContext) : null;
        }

        [DoesServiceRequest]
        public virtual async Task<ServiceStats> GetServiceStatsAsync(
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            return _client != null
                ? await _client.GetServiceStatsAsync(options, operationContext, cancellationToken)
                : null;
        }

        public void ResetCredentials(StorageCredentials tokenCredential)
        {
            _logger.LogDebug($"Resetting storage account {AccountName} credentials.");
            _client = new CloudBlobClient(BaseUri, tokenCredential);
        }
    }
}