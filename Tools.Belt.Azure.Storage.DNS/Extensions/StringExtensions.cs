using System;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.Storage.DNS.Extensions
{
    public static class StringExtensions
    {
        public static Uri ToBlobStorageAccountUri(this string storageAccountName)
        {
            if (storageAccountName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(storageAccountName));
            return new Uri($"https://{storageAccountName}.blob.core.windows.net");
        }

        public static Uri ToTableStorageAccountUri(this string storageAccountName)
        {
            if (storageAccountName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(storageAccountName));
            return new Uri($"https://{storageAccountName}.table.core.windows.net");
        }
    }
}