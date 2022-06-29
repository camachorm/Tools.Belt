using System;

namespace Tools.Belt.Azure.DNS.Extensions
{
    public static class StorageAccountExtensions
    {
        public static string ParseStorageAccountNameFromConnectionString(this string source)
        {
            const string accountField = "AccountName=";

            if (!source.Contains(accountField)) return source;
            int accountFieldIndex = source.IndexOf(accountField, StringComparison.InvariantCultureIgnoreCase);
            int startIndex = accountFieldIndex + accountField.Length;
            int endIndex = source.IndexOf(';', startIndex);
            return source.Substring(startIndex, endIndex - startIndex);
        }
    }
}