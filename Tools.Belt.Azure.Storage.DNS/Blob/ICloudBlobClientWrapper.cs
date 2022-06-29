using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public interface ICloudBlobClientWrapper
    {
        string AccountName { get; }

        IBufferManager BufferManager { get; set; }
        StorageCredentials Credentials { get; }
        Uri BaseUri { get; }
        StorageUri StorageUri { get; }
        BlobRequestOptions DefaultRequestOptions { get; set; }
        IRetryPolicy RetryPolicy { get; set; }
        string DefaultDelimiter { get; set; }
        AuthenticationScheme AuthenticationScheme { get; set; }
        CloudBlobContainer GetRootContainerReference();
        CloudBlobContainer GetContainerReference(string containerName);

        Task<ContainerResultSegment> ListContainersSegmentedAsync(
            BlobContinuationToken currentToken);

        Task<ContainerResultSegment> ListContainersSegmentedAsync(
            string prefix,
            BlobContinuationToken currentToken);

        Task<ContainerResultSegment> ListContainersSegmentedAsync(
            string prefix,
            ContainerListingDetails detailsIncluded,
            int? maxResults,
            BlobContinuationToken currentToken,
            BlobRequestOptions options,
            OperationContext operationContext);

        Task<ContainerResultSegment> ListContainersSegmentedAsync(
            string prefix,
            ContainerListingDetails detailsIncluded,
            int? maxResults,
            BlobContinuationToken currentToken,
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<BlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            BlobContinuationToken currentToken);

        Task<BlobResultSegment> ListBlobsSegmentedAsync(
            string prefix,
            bool useFlatBlobListing,
            BlobListingDetails blobListingDetails,
            int? maxResults,
            BlobContinuationToken currentToken,
            BlobRequestOptions options,
            OperationContext operationContext);

        Task<ICloudBlob> GetBlobReferenceFromServerAsync(Uri blobUri);

        Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            Uri blobUri,
            AccessCondition accessCondition,
            BlobRequestOptions options,
            OperationContext operationContext);

        Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            StorageUri blobUri,
            AccessCondition accessCondition,
            BlobRequestOptions options,
            OperationContext operationContext);

        Task<ICloudBlob> GetBlobReferenceFromServerAsync(
            StorageUri blobUri,
            AccessCondition accessCondition,
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<AccountProperties> GetAccountPropertiesAsync();

        Task<AccountProperties> GetAccountPropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext);

        Task<AccountProperties> GetAccountPropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<ServiceProperties> GetServicePropertiesAsync();

        Task<ServiceProperties> GetServicePropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext);

        Task<ServiceProperties> GetServicePropertiesAsync(
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task SetServicePropertiesAsync(ServiceProperties properties);

        Task SetServicePropertiesAsync(
            ServiceProperties properties,
            BlobRequestOptions requestOptions,
            OperationContext operationContext);

        Task SetServicePropertiesAsync(
            ServiceProperties properties,
            BlobRequestOptions requestOptions,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        Task<ServiceStats> GetServiceStatsAsync();

        Task<ServiceStats> GetServiceStatsAsync(
            BlobRequestOptions options,
            OperationContext operationContext);

        Task<ServiceStats> GetServiceStatsAsync(
            BlobRequestOptions options,
            OperationContext operationContext,
            CancellationToken cancellationToken);

        void ResetCredentials(StorageCredentials tokenCredential);
    }
}