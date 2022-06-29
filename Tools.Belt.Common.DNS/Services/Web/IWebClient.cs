using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Belt.Common.Services.Web
{
    public interface IWebClient
    {
        /// <inheritdoc cref="WebClient" />
        string BaseAddress { get; set; }

        /// <inheritdoc cref="WebClient" />
        RequestCachePolicy CachePolicy { get; set; }

        /// <inheritdoc cref="WebClient" />
        ICredentials Credentials { get; set; }

        /// <inheritdoc cref="WebClient" />
        Encoding Encoding { get; set; }


        /// <inheritdoc cref="WebClient" />
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
            Justification = "Wrapper property.")]
        WebHeaderCollection Headers { get; set; }

        /// <inheritdoc cref="WebClient" />
        bool IsBusy { get; }

        /// <inheritdoc cref="WebClient" />
        IWebProxy Proxy { get; set; }


        /// <inheritdoc cref="WebClient" />
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
            Justification = "Wrapper property.")]
        NameValueCollection QueryString { get; set; }

        /// <inheritdoc cref="WebClient" />
        WebHeaderCollection ResponseHeaders { get; }

        /// <inheritdoc cref="WebClient" />
        bool UseDefaultCredentials { get; set; }

        /// <inheritdoc cref="WebClient" />
        event DownloadDataCompletedEventHandler DownloadDataCompleted;

        /// <inheritdoc cref="WebClient" />
        event AsyncCompletedEventHandler DownloadFileCompleted;

        /// <inheritdoc cref="WebClient" />
        event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <inheritdoc cref="WebClient" />
        event DownloadStringCompletedEventHandler DownloadStringCompleted;

        /// <inheritdoc cref="WebClient" />
        event OpenReadCompletedEventHandler OpenReadCompleted;

        /// <inheritdoc cref="WebClient" />
        event OpenWriteCompletedEventHandler OpenWriteCompleted;

        /// <inheritdoc cref="WebClient" />
        event UploadDataCompletedEventHandler UploadDataCompleted;

        /// <inheritdoc cref="WebClient" />
        event UploadFileCompletedEventHandler UploadFileCompleted;

        /// <inheritdoc cref="WebClient" />
        event UploadProgressChangedEventHandler UploadProgressChanged;

        /// <inheritdoc cref="WebClient" />
        event UploadStringCompletedEventHandler UploadStringCompleted;

        /// <inheritdoc cref="WebClient" />
        event UploadValuesCompletedEventHandler UploadValuesCompleted;

        /// <summary>Cancels a pending asynchronous operation.</summary>
        void CancelAsync();

        /// <inheritdoc cref="WebClient" />
        byte[] DownloadData(string address);

        /// <inheritdoc cref="WebClient" />
        byte[] DownloadData(Uri address);

        /// <inheritdoc cref="WebClient" />
        void DownloadDataAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        void DownloadDataAsync(Uri address, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> DownloadDataTaskAsync(string address);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> DownloadDataTaskAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        void DownloadFile(string address, string fileName);

        /// <inheritdoc cref="WebClient" />
        void DownloadFile(Uri address, string fileName);

        /// <inheritdoc cref="WebClient" />
        void DownloadFileAsync(Uri address, string fileName);

        /// <inheritdoc cref="WebClient" />
        void DownloadFileAsync(Uri address, string fileName, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task DownloadFileTaskAsync(string address, string fileName);

        /// <inheritdoc cref="WebClient" />
        Task DownloadFileTaskAsync(Uri address, string fileName);

        /// <inheritdoc cref="WebClient" />
        string DownloadString(string address);

        /// <inheritdoc cref="WebClient" />
        string DownloadString(Uri address);

        /// <inheritdoc cref="WebClient" />
        void DownloadStringAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        void DownloadStringAsync(Uri address, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<string> DownloadStringTaskAsync(string address);

        /// <inheritdoc cref="WebClient" />
        Task<string> DownloadStringTaskAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        Stream OpenRead(string address);

        /// <inheritdoc cref="WebClient" />
        Stream OpenRead(Uri address);

        /// <inheritdoc cref="WebClient" />
        void OpenReadAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        void OpenReadAsync(Uri address, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<Stream> OpenReadTaskAsync(string address);

        /// <inheritdoc cref="WebClient" />
        Task<Stream> OpenReadTaskAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        Stream OpenWrite(string address);

        /// <inheritdoc cref="WebClient" />
        Stream OpenWrite(string address, string method);

        /// <inheritdoc cref="WebClient" />
        Stream OpenWrite(Uri address);

        /// <inheritdoc cref="WebClient" />
        Stream OpenWrite(Uri address, string method);

        /// <inheritdoc cref="WebClient" />
        void OpenWriteAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        void OpenWriteAsync(Uri address, string method);

        /// <inheritdoc cref="WebClient" />
        void OpenWriteAsync(Uri address, string method, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<Stream> OpenWriteTaskAsync(string address);

        /// <inheritdoc cref="WebClient" />
        Task<Stream> OpenWriteTaskAsync(string address, string method);

        /// <inheritdoc cref="WebClient" />
        Task<Stream> OpenWriteTaskAsync(Uri address);

        /// <inheritdoc cref="WebClient" />
        Task<Stream> OpenWriteTaskAsync(Uri address, string method);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadData(string address, byte[] data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadData(string address, string method, byte[] data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadData(Uri address, byte[] data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadData(Uri address, string method, byte[] data);

        /// <inheritdoc cref="WebClient" />
        void UploadDataAsync(Uri address, byte[] data);

        /// <inheritdoc cref="WebClient" />
        void UploadDataAsync(Uri address, string method, byte[] data);

        /// <inheritdoc cref="WebClient" />
        void UploadDataAsync(Uri address, string method, byte[] data, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadDataTaskAsync(string address, byte[] data);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadFile(string address, string fileName);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadFile(string address, string method, string fileName);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadFile(Uri address, string fileName);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadFile(Uri address, string method, string fileName);

        /// <inheritdoc cref="WebClient" />
        void UploadFileAsync(Uri address, string fileName);

        /// <inheritdoc cref="WebClient" />
        void UploadFileAsync(Uri address, string method, string fileName);

        /// <inheritdoc cref="WebClient" />
        void UploadFileAsync(Uri address, string method, string fileName, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadFileTaskAsync(string address, string fileName);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadFileTaskAsync(Uri address, string fileName);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName);

        /// <inheritdoc cref="WebClient" />
        string UploadString(string address, string data);

        /// <inheritdoc cref="WebClient" />
        string UploadString(string address, string method, string data);

        /// <inheritdoc cref="WebClient" />
        string UploadString(Uri address, string data);

        /// <inheritdoc cref="WebClient" />
        string UploadString(Uri address, string method, string data);

        /// <inheritdoc cref="WebClient" />
        void UploadStringAsync(Uri address, string data);

        /// <inheritdoc cref="WebClient" />
        void UploadStringAsync(Uri address, string method, string data);

        /// <inheritdoc cref="WebClient" />
        void UploadStringAsync(Uri address, string method, string data, object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<string> UploadStringTaskAsync(string address, string data);

        /// <inheritdoc cref="WebClient" />
        Task<string> UploadStringTaskAsync(string address, string method, string data);

        /// <inheritdoc cref="WebClient" />
        Task<string> UploadStringTaskAsync(Uri address, string data);

        /// <inheritdoc cref="WebClient" />
        Task<string> UploadStringTaskAsync(Uri address, string method, string data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadValues(string address, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadValues(string address, string method, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadValues(Uri address, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        byte[] UploadValues(Uri address, string method, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        void UploadValuesAsync(Uri address, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        void UploadValuesAsync(Uri address, string method, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        void UploadValuesAsync(
            Uri address,
            string method,
            NameValueCollection data,
            object userToken);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadValuesTaskAsync(
            string address,
            string method,
            NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data);

        /// <inheritdoc cref="WebClient" />
        Task<byte[]> UploadValuesTaskAsync(Uri address, string method, NameValueCollection data);
    }
}