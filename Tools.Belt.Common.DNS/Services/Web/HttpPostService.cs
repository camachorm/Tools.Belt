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
    public class HttpPostService : HttpPutPostBase, IHttpPostService
    {
        public HttpPostService() : this(new WebClient())
        {
        }

        public HttpPostService(WebClientWrapper client) : base(client)
        {
        }

        protected override string Verb { get; } = "POST";

        public async Task<ToolsBeltHttpResponse<string>> PostJsonAsync(
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

        public async Task<ToolsBeltHttpResponse<string>> PostJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JToken messageData)
        {
            return await PostJsonAsync(logger, config, endpointUrl, new List<KeyValuePair<string, string>>(),
                messageData.ToJObject()).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> PostStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            string messageData)
        {
            return await ExecuteAsync(logger, config, endpointUrl, headers, messageData).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PostDataAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            byte[] messageData)
        {
            return await ExecuteAsync(logger, config, endpointUrl, headers, messageData).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PostValuesAsync(
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