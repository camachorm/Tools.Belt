using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services.Web
{
    [ExcludeFromCodeCoverage]
    public abstract class HttpPutPostBase : HttpServiceBase
    {
        protected HttpPutPostBase()
        {
        }

        protected HttpPutPostBase(WebClientWrapper client) : base(client)
        {
            UploadValues = (uri, messageData) => Client.UploadValues(uri, Verb, messageData);

            UploadString = (uri, messageData) => Client.UploadString(uri, Verb, messageData);

            UploadData = (uri, messageData) => Client.UploadData(uri, Verb, messageData);
        }

        protected Func<Uri, byte[], byte[]> UploadData { get; }

        protected Func<Uri, string, string> UploadString { get; }

        protected Func<Uri, NameValueCollection, byte[]> UploadValues { get; }

        protected abstract string Verb { get; }


        #region IService Implementations

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            string messageData)
        {
            return await GenericRequest(logger, config, headers, endpointUrl,
                () => UploadString(endpointUrl, messageData)).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            byte[] messageData)
        {
            return await GenericRequest(logger, config, headers, endpointUrl,
                () => UploadData(endpointUrl, messageData)).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await GenericRequest(logger, config, headers, endpointUrl,
                () => UploadValues(endpointUrl, messageData)).ConfigureAwait(false);
        }

        #endregion
    }
}