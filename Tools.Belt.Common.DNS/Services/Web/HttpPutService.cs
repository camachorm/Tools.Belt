using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Services.Web
{
    [ExcludeFromCodeCoverage]
    public class HttpPutService : HttpPutPostBase, IHttpPutService
    {
        public HttpPutService() : this(new WebClient())
        {
        }

        public HttpPutService(WebClient client) : base(client)
        {
        }

        protected override string Verb { get; } = "PUT";

        public async Task<ToolsBeltHttpResponse<string>> PutJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JObject messageData)
        {
            if (messageData == null) throw new ArgumentNullException(nameof(messageData));
            return await ExecuteAsync(
                logger,
                config,
                endpointUrl,
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Content-Type", "application/json")
                },
                messageData.ToString()).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> PutJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JToken messageData)
        {
            return await PutJsonAsync(logger, config, endpointUrl, new List<KeyValuePair<string, string>>(),
                messageData.ToJObject()).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> PutStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            string messageData)
        {
            return await ExecuteAsync(logger, config, endpointUrl, headers, messageData).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PutDataAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            byte[] messageData)
        {
            return await ExecuteAsync(logger, config, endpointUrl, headers, messageData).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PutValuesAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await ExecuteAsync(logger, config, endpointUrl, headers, messageData).ConfigureAwait(false);
        }
    }
}