using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services.Web
{
    public interface IHttpGetService :
        IService<ToolsBeltHttpResponse<string>, Uri, IEnumerable<KeyValuePair<string, string>>
        >, // Simple Get, no parameters
        IService<ToolsBeltHttpResponse<string>, Uri, IEnumerable<KeyValuePair<string, string>>, NameValueCollection
        >, // Simple Get with query string params
        IService<ToolsBeltHttpResponse<string>, Uri, IEnumerable<KeyValuePair<string, string>>,
            IEnumerable<Tuple<string, string>>>, // Simple Get with query string params
        IService<ToolsBeltHttpResponse<string>, Uri, IEnumerable<KeyValuePair<string, string>>,
            Dictionary<string, string>> // Simple Get with query string params
    {
        Task<ToolsBeltHttpResponse<string>> GetStringAsync(ILogger logger, IConfigurationService config,
            Uri endpointUrl, IEnumerable<KeyValuePair<string, string>> headers);

        Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            IEnumerable<Tuple<string, string>> queryParams);

        Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            Dictionary<string, string> queryParams);

        Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection queryParams);
    }
}