using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Tools.Belt.Azure.Storage.DNS.Table;

namespace Tools.Belt.Azure.Storage.DNS.Common
{
    public class CloudStorageAccountWrapper :  ICloudStorageAccountWrapper
    {
        private readonly CloudStorageAccount _account;

        public CloudStorageAccountWrapper(StorageCredentials storageCredentials, Uri blobEndpoint, Uri queueEndpoint, Uri tableEndpoint, Uri fileEndpoint) 
        {
            _account = new CloudStorageAccount(storageCredentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
        }

        public CloudStorageAccountWrapper(StorageCredentials storageCredentials, StorageUri blobStorageUri, StorageUri queueStorageUri, StorageUri tableStorageUri, StorageUri fileStorageUri) 
        {
            _account = new CloudStorageAccount(storageCredentials, blobStorageUri, queueStorageUri, tableStorageUri, fileStorageUri);
        }

        public CloudStorageAccountWrapper(StorageCredentials storageCredentials, bool useHttps)
        {
            _account = new CloudStorageAccount(storageCredentials, useHttps);
        }

        public CloudStorageAccountWrapper(StorageCredentials storageCredentials, string endpointSuffix, bool useHttps)
        {
            _account=new CloudStorageAccount(storageCredentials, endpointSuffix, useHttps);
        }

        public CloudStorageAccountWrapper(StorageCredentials storageCredentials, string accountName, string endpointSuffix, bool useHttps) 
        {
            _account = new CloudStorageAccount(storageCredentials, accountName, endpointSuffix, useHttps);
        }

        public CloudStorageAccountWrapper(CloudStorageAccount cloudStorageAccount)
        {
            _account = cloudStorageAccount;
        }

        public CloudStorageAccountWrapper()
        {



        }

        public Uri BlobEndpoint => _account?.BlobEndpoint;
        public Uri QueueEndpoint => _account?.QueueEndpoint;
        public Uri TableEndpoint => _account?.TableEndpoint;
        public Uri FileEndpoint => _account?.FileEndpoint;
        public StorageUri BlobStorageUri => _account?.BlobStorageUri;
        public StorageUri QueueStorageUri => _account?.QueueStorageUri;
        public StorageUri TableStorageUri => _account?.TableStorageUri;
        public StorageUri FileStorageUri => _account?.FileStorageUri;
        public StorageCredentials Credentials => _account?.Credentials;
        public ICloudTableClientWrapper CreateCloudTableClient()
        {
            return new CloudTableClientWrapper(_account?.CreateCloudTableClient());
        }

        public CloudQueueClient CreateCloudQueueClient()
        {
            throw new NotImplementedException();
        }

        public CloudBlobClient CreateCloudBlobClient()
        {
            throw new NotImplementedException();
        }

        public CloudFileClient CreateCloudFileClient()
        {
            throw new NotImplementedException();
        }

        public string GetSharedAccessSignature(SharedAccessAccountPolicy policy)
        {
            return _account.GetSharedAccessSignature(policy);
        }

        public string ToString(bool exportSecrets)
        {
            throw new NotImplementedException();
        }
    }
}