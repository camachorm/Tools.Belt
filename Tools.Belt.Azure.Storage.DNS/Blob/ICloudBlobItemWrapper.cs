using System;
using Microsoft.Azure.Storage.Blob;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public interface ICloudBlobItemWrapper : IListBlobItem
    {
        string Name { get; }
        DateTimeOffset? CreatedAt { get; }
        DateTimeOffset? ModifiedAt { get; }
        DateTimeOffset? DeletedAt { get; }
        BlobType? BlobType { get; }
    }
}