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
    [ExcludeFromCodeCoverage]
    public class WebClientWrapper : IWebClient
    {
        private readonly WebClient _client;

        public WebClientWrapper(WebClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public WebClientWrapper() : this(new WebClient())
        {
        }

        public virtual string BaseAddress
        {
            get => _client?.BaseAddress;
            set => _client.BaseAddress = value;
        }

        public virtual RequestCachePolicy CachePolicy
        {
            get => _client?.CachePolicy;
            set => _client.CachePolicy = value;
        }

        public virtual ICredentials Credentials
        {
            get => _client?.Credentials;
            set => _client.Credentials = value;
        }

        public virtual Encoding Encoding
        {
            get => _client?.Encoding;
            set => _client.Encoding = value;
        }


        [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
            Justification = "Wrapper property must reflect original property capabilities.")]
        public virtual WebHeaderCollection Headers
        {
            get => _client?.Headers;
            set => _client.Headers = value;
        }

        public virtual bool IsBusy => _client.IsBusy;

        public virtual IWebProxy Proxy
        {
            get => _client?.Proxy;
            set => _client.Proxy = value;
        }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
            Justification = "Wrapper property must reflect original property capabilities.")]
        public virtual NameValueCollection QueryString
        {
            get => _client?.QueryString;
            set => _client.QueryString = value;
        }

        public virtual WebHeaderCollection ResponseHeaders => _client?.ResponseHeaders;

        public virtual bool UseDefaultCredentials
        {
            get => _client.UseDefaultCredentials;
            set => _client.UseDefaultCredentials = value;
        }

        public virtual void CancelAsync()
        {
            _client.CancelAsync();
        }

        public virtual byte[] DownloadData(string address)
        {
            return _client.DownloadData(address);
        }

        public virtual byte[] DownloadData(Uri address)
        {
            return _client.DownloadData(address);
        }

        public virtual void DownloadDataAsync(Uri address)
        {
            _client.DownloadDataAsync(address);
        }

        public virtual void DownloadDataAsync(Uri address, object userToken)
        {
            _client.DownloadDataAsync(address, userToken);
        }

        public virtual async Task<byte[]> DownloadDataTaskAsync(string address)
        {
            return await _client.DownloadDataTaskAsync(address).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> DownloadDataTaskAsync(Uri address)
        {
            return await _client.DownloadDataTaskAsync(address).ConfigureAwait(false);
        }

        public virtual void DownloadFile(string address, string fileName)
        {
            _client.DownloadFile(address, fileName);
        }

        public virtual void DownloadFile(Uri address, string fileName)
        {
            _client.DownloadFile(address, fileName);
        }

        public virtual void DownloadFileAsync(Uri address, string fileName)
        {
            _client.DownloadFileAsync(address, fileName);
        }

        public virtual void DownloadFileAsync(Uri address, string fileName, object userToken)
        {
            _client.DownloadFileAsync(address, fileName);
        }

        public virtual async Task DownloadFileTaskAsync(string address, string fileName)
        {
            await _client.DownloadFileTaskAsync(address, fileName).ConfigureAwait(false);
        }

        public virtual async Task DownloadFileTaskAsync(Uri address, string fileName)
        {
            await _client.DownloadFileTaskAsync(address, fileName).ConfigureAwait(false);
        }

        public virtual string DownloadString(string address)
        {
            return _client.DownloadString(address);
        }

        public virtual string DownloadString(Uri address)
        {
            return _client.DownloadString(address);
        }

        public virtual void DownloadStringAsync(Uri address)
        {
            _client.DownloadString(address);
        }

        public virtual void DownloadStringAsync(Uri address, object userToken)
        {
            _client.DownloadStringAsync(address, userToken);
        }

        public virtual async Task<string> DownloadStringTaskAsync(string address)
        {
            return await _client.DownloadStringTaskAsync(address).ConfigureAwait(false);
        }

        public virtual async Task<string> DownloadStringTaskAsync(Uri address)
        {
            return await _client.DownloadStringTaskAsync(address).ConfigureAwait(false);
        }

        public virtual Stream OpenRead(string address)
        {
            return _client.OpenRead(address);
        }

        public virtual Stream OpenRead(Uri address)
        {
            return _client.OpenRead(address);
        }

        public virtual void OpenReadAsync(Uri address)
        {
            _client.OpenReadAsync(address);
        }

        public virtual void OpenReadAsync(Uri address, object userToken)
        {
            _client.OpenReadAsync(address, userToken);
        }

        public virtual async Task<Stream> OpenReadTaskAsync(string address)
        {
            return await _client.OpenReadTaskAsync(address).ConfigureAwait(false);
        }

        public virtual async Task<Stream> OpenReadTaskAsync(Uri address)
        {
            return await _client.OpenReadTaskAsync(address).ConfigureAwait(false);
        }

        public virtual Stream OpenWrite(string address)
        {
            return _client.OpenWrite(address);
        }

        public virtual Stream OpenWrite(string address, string method)
        {
            return _client.OpenWrite(address, method);
        }

        public virtual Stream OpenWrite(Uri address)
        {
            return _client.OpenWrite(address);
        }

        public virtual Stream OpenWrite(Uri address, string method)
        {
            return _client.OpenWrite(address, method);
        }

        public virtual void OpenWriteAsync(Uri address)
        {
            _client.OpenWriteAsync(address);
        }

        public virtual void OpenWriteAsync(Uri address, string method)
        {
            _client.OpenWriteAsync(address, method);
        }

        public virtual void OpenWriteAsync(Uri address, string method, object userToken)
        {
            _client.OpenWriteAsync(address, method, userToken);
        }

        public virtual async Task<Stream> OpenWriteTaskAsync(string address)
        {
            return await _client.OpenWriteTaskAsync(address).ConfigureAwait(false);
        }

        public virtual async Task<Stream> OpenWriteTaskAsync(string address, string method)
        {
            return await _client.OpenWriteTaskAsync(address, method).ConfigureAwait(false);
        }

        public virtual async Task<Stream> OpenWriteTaskAsync(Uri address)
        {
            return await _client.OpenWriteTaskAsync(address).ConfigureAwait(false);
        }

        public virtual async Task<Stream> OpenWriteTaskAsync(Uri address, string method)
        {
            return await _client.OpenWriteTaskAsync(address, method).ConfigureAwait(false);
        }

        public virtual byte[] UploadData(string address, byte[] data)
        {
            return _client.UploadData(address, data);
        }

        public virtual byte[] UploadData(string address, string method, byte[] data)
        {
            return _client.UploadData(address, method, data);
        }

        public virtual byte[] UploadData(Uri address, byte[] data)
        {
            return _client.UploadData(address, data);
        }

        public virtual byte[] UploadData(Uri address, string method, byte[] data)
        {
            return _client.UploadData(address, method, data);
        }

        public virtual void UploadDataAsync(Uri address, byte[] data)
        {
            _client.UploadDataAsync(address, data);
        }

        public virtual void UploadDataAsync(Uri address, string method, byte[] data)
        {
            _client.UploadDataAsync(address, method, data);
        }

        public virtual void UploadDataAsync(Uri address, string method, byte[] data, object userToken)
        {
            _client.UploadDataAsync(address, method, data, userToken);
        }

        public virtual async Task<byte[]> UploadDataTaskAsync(string address, byte[] data)
        {
            return await _client.UploadDataTaskAsync(address, data).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data)
        {
            return await _client.UploadDataTaskAsync(address, method, data).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data)
        {
            return await _client.UploadDataTaskAsync(address, data).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data)
        {
            return await _client.UploadDataTaskAsync(address, method, data).ConfigureAwait(false);
        }

        public virtual byte[] UploadFile(string address, string fileName)
        {
            return _client.UploadFile(address, fileName);
        }

        public virtual byte[] UploadFile(string address, string method, string fileName)
        {
            return _client.UploadFile(address, method, fileName);
        }

        public virtual byte[] UploadFile(Uri address, string fileName)
        {
            return _client.UploadFile(address, fileName);
        }

        public virtual byte[] UploadFile(Uri address, string method, string fileName)
        {
            return _client.UploadFile(address, method, fileName);
        }

        public virtual void UploadFileAsync(Uri address, string fileName)
        {
            _client.UploadFileAsync(address, fileName);
        }

        public virtual void UploadFileAsync(Uri address, string method, string fileName)
        {
            _client.UploadFileAsync(address, method, fileName);
        }

        public virtual void UploadFileAsync(Uri address, string method, string fileName, object userToken)
        {
            _client.UploadFileAsync(address, method, fileName, userToken);
        }

        public virtual async Task<byte[]> UploadFileTaskAsync(string address, string fileName)
        {
            return await _client.UploadFileTaskAsync(address, fileName).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName)
        {
            return await _client.UploadFileTaskAsync(address, method, fileName).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadFileTaskAsync(Uri address, string fileName)
        {
            return await _client.UploadFileTaskAsync(address, fileName).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName)
        {
            return await _client.UploadFileTaskAsync(address, method, fileName).ConfigureAwait(false);
        }

        public virtual string UploadString(string address, string data)
        {
            return _client.UploadString(address, data);
        }

        public virtual string UploadString(string address, string method, string data)
        {
            return _client.UploadString(address, method, data);
        }

        public virtual string UploadString(Uri address, string data)
        {
            return _client.UploadString(address, data);
        }

        public virtual string UploadString(Uri address, string method, string data)
        {
            return _client.UploadString(address, method, data);
        }

        public virtual void UploadStringAsync(Uri address, string data)
        {
            _client.UploadStringAsync(address, data);
        }

        public virtual void UploadStringAsync(Uri address, string method, string data)
        {
            _client.UploadStringAsync(address, method, data);
        }

        public virtual void UploadStringAsync(Uri address, string method, string data, object userToken)
        {
            _client.UploadStringAsync(address, method, data, userToken);
        }

        public virtual async Task<string> UploadStringTaskAsync(string address, string data)
        {
            return await _client.UploadStringTaskAsync(address, data).ConfigureAwait(false);
        }

        public virtual async Task<string> UploadStringTaskAsync(string address, string method, string data)
        {
            return await _client.UploadStringTaskAsync(address, method, data).ConfigureAwait(false);
        }

        public virtual async Task<string> UploadStringTaskAsync(Uri address, string data)
        {
            return await _client.UploadStringTaskAsync(address, data).ConfigureAwait(false);
        }

        public virtual async Task<string> UploadStringTaskAsync(Uri address, string method, string data)
        {
            return await _client.UploadStringTaskAsync(address, method, data).ConfigureAwait(false);
        }

        public virtual byte[] UploadValues(string address, NameValueCollection data)
        {
            return _client.UploadValues(address, data);
        }

        public virtual byte[] UploadValues(string address, string method, NameValueCollection data)
        {
            return _client.UploadValues(address, method, data);
        }

        public virtual byte[] UploadValues(Uri address, NameValueCollection data)
        {
            return _client.UploadValues(address, data);
        }

        public virtual byte[] UploadValues(Uri address, string method, NameValueCollection data)
        {
            return _client.UploadValues(address, method, data);
        }

        public virtual void UploadValuesAsync(Uri address, NameValueCollection data)
        {
            _client.UploadValuesAsync(address, data);
        }

        public virtual void UploadValuesAsync(Uri address, string method, NameValueCollection data)
        {
            _client.UploadValuesAsync(address, method, data);
        }

        public virtual void UploadValuesAsync(Uri address, string method, NameValueCollection data, object userToken)
        {
            _client.UploadValuesAsync(address, method, data, userToken);
        }

        public virtual async Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data)
        {
            return await _client.UploadValuesTaskAsync(address, data).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadValuesTaskAsync(string address, string method, NameValueCollection data)
        {
            return await _client.UploadValuesTaskAsync(address, method, data).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data)
        {
            return await _client.UploadValuesTaskAsync(address, data).ConfigureAwait(false);
        }

        public virtual async Task<byte[]> UploadValuesTaskAsync(Uri address, string method, NameValueCollection data)
        {
            return await _client.UploadValuesTaskAsync(address, method, data).ConfigureAwait(false);
        }

        public static WebClientWrapper FromWebClient(WebClient client)
        {
            return new WebClientWrapper(client);
        }

        public static implicit operator WebClientWrapper(WebClient client)
        {
            return new WebClientWrapper(client);
        }
#pragma warning disable 67
        public virtual event DownloadDataCompletedEventHandler DownloadDataCompleted;
        public virtual event AsyncCompletedEventHandler DownloadFileCompleted;
        public virtual event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public virtual event DownloadStringCompletedEventHandler DownloadStringCompleted;
        public virtual event OpenReadCompletedEventHandler OpenReadCompleted;
        public virtual event OpenWriteCompletedEventHandler OpenWriteCompleted;
        public virtual event UploadDataCompletedEventHandler UploadDataCompleted;
        public virtual event UploadFileCompletedEventHandler UploadFileCompleted;
        public virtual event UploadProgressChangedEventHandler UploadProgressChanged;
        public virtual event UploadStringCompletedEventHandler UploadStringCompleted;
        public virtual event UploadValuesCompletedEventHandler UploadValuesCompleted;
#pragma warning restore 67
    }
}