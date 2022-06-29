using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Tools.Belt.Azure.Storage.DNS.Table;

namespace Tools.Belt.Azure.Storage.DNS.Common
{
    public interface ICloudStorageAccountWrapper
    {
        Uri BlobEndpoint { get; }
        Uri QueueEndpoint { get; }
        Uri TableEndpoint { get; }
        Uri FileEndpoint { get; }
        StorageUri BlobStorageUri { get; }
        StorageUri QueueStorageUri { get; }
        StorageUri TableStorageUri { get; }
        StorageUri FileStorageUri { get; }
        StorageCredentials Credentials { get; } 
        ICloudTableClientWrapper CreateCloudTableClient();
        CloudQueueClient CreateCloudQueueClient();
        CloudBlobClient CreateCloudBlobClient();
        CloudFileClient CreateCloudFileClient();
        string GetSharedAccessSignature(SharedAccessAccountPolicy policy);
        string ToString();
        string ToString(bool exportSecrets);
    }
}