using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services.Web
{
    public interface IHttpPostService
    {
        Task<ToolsBeltHttpResponse<string>> PostJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JObject messageData);

        Task<ToolsBeltHttpResponse<string>> PostJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JToken messageData);

        Task<ToolsBeltHttpResponse<string>> PostStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            string messageData);

        Task<ToolsBeltHttpResponse<byte[]>> PostDataAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            byte[] messageData);

        Task<ToolsBeltHttpResponse<byte[]>> PostValuesAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData);
    }
}