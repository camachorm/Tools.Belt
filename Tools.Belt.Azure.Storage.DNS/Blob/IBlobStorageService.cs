using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Tools.Belt.Azure.Storage.DNS.Common;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public interface IBlobStorageService : IStorageService
    {
        string ConnString { get; }

        string AccountName { get; }

        ICloudBlobClientWrapper ClientWrapper { get; }

        Task<bool> CreateContainerPathAsync(string path);
        bool CreateContainerPath(string path);
        IEnumerable<ICloudBlobItemWrapper> ListAllBlobs(string rootContainer, string path);
        Task<IEnumerable<ICloudBlobItemWrapper>> ListAllBlobsAsync(string rootContainer, string path);
        Task<IEnumerable<ICloudBlobItemWrapper>> ListAllBlobsAsync(string rootContainer, string path, BlobListingDetails blobListingDetails);
        byte[] ReadFileAsByteArray(string containerPath, string fileName);
        Task<byte[]> ReadFileAsByteArrayAsync(string containerPath, string fileName);
        string ReadFileAsString(string containerPath, string fileName);
        Task<string> ReadFileAsStringAsync(string containerPath, string fileName);
        Task<Stream> ReadFileAsStreamAsync(string containerPath, string fileName);

        Stream OpenRead(string containerPath, string fileName);

        Task<Stream> OpenReadAsync(string containerPath, string fileName);

        Stream OpenWrite(string containerPath, string fileName);

        Task<Stream> OpenWriteAsync(string containerPath, string fileName);

        void ReadFileIntoFile(string containerPath, string sourceFileName, string destinationFileFullPath,
            FileMode fileMode = FileMode.Create);

        Task ReadFileIntoFileAsync(string containerPath, string sourceFileName, string destinationFileFullPath,
            FileMode fileMode = FileMode.Create);

        void WriteFileAsBlockBlob(string containerPath, string fileName, byte[] fileContent);
        void WriteFileAsBlockBlob(string containerPath, string fileName, Stream fileContent);
        void WriteFileAsBlockBlob(string containerPath, string fileName, string fileContent);
        Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, byte[] fileContent);
        Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, Stream fileContent);

        Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, Stream fileContent, string leaseId);

        Task WriteFileAsBlockBlobAsync(string containerPath, string fileName, string fileContent);

        Task<string> AcquireLeaseAsync(string containerPath, string leaseFullName, TimeSpan? duration = null,
            string proposedLeaseId = null, byte[] defaultContent = null, bool createIfNotExist = true);

        Task<string> TryAcquireLeaseAsync(string containerPath, string leaseFullName, TimeSpan? duration = null,
            string proposedLeaseId = null);

        Task ReleaseLeaseAsync(string containerPath, string leaseFullName, string leaseId);
        Task<bool> FileExistsAsync(string containerPath, string fileName);
        Task<bool> DeleteIfExistsAsync(string containerPath, string fileName);
        Task CreateOrAppendFileAsBlockBlobAsync(string containerPath, string fileName, byte[] fileContent);
        Task CopyBlobAsync(string containerPath, string fileName, string destinationContainerPath,
            string destinationFileName);

        Task<BlobProperties> GetBlobProperties(string containerPath, string filename);
    }
}