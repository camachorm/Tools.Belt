using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Tools.Belt.Azure.Storage.DNS.Blob
{
    public class CloudBlobItemWrapper : ICloudBlobItemWrapper
    {
        private readonly IListBlobItem _item;

        public CloudBlobItemWrapper(IListBlobItem item)
        {
            _item = item;
        }

        public CloudBlobContainer Container => _item?.Container;
        public CloudBlobDirectory Parent => _item?.Parent;
        public StorageUri StorageUri => _item?.StorageUri;
        public Uri Uri => _item?.Uri;

        public string Name => _item is CloudBlob blob ? blob.Name : null;
        public DateTimeOffset? CreatedAt => _item is CloudBlob blob ? blob.Properties.Created : null;
        public DateTimeOffset? ModifiedAt => _item is CloudBlob blob ? blob.Properties.LastModified : null;
        public DateTimeOffset? DeletedAt => _item is CloudBlob blob ? blob.Properties.DeletedTime : null;
        public BlobType? BlobType => _item is CloudBlob blob ? blob.Properties.BlobType : (BlobType?) null;
    }
}