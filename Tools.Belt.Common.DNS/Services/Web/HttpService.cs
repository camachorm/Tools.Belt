using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services.Web
{
    [ExcludeFromCodeCoverage]
    public class HttpService : IHttpService
    {
        private readonly IHttpPutService _putService;

        public HttpService() : this(new HttpPostService(), new HttpGetService(), new HttpPutService())
        {
        }

        public HttpService(IHttpPostService postService, IHttpGetService getService, IHttpPutService putService)
        {
            _putService = putService;
            PostService = postService;
            GetService = getService;
        }

        // ReSharper disable MemberCanBePrivate.Global
        public IHttpPostService PostService { get; }

        public IHttpGetService GetService { get; }
        // ReSharper restore MemberCanBePrivate.Global

        #region IHttpPostService Implementation

        public async Task<ToolsBeltHttpResponse<string>> PostJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JObject messageData)
        {
            return await PostService.PostJsonAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> PostJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JToken messageData)
        {
            return await PostService.PostJsonAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> PostStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            string messageData)
        {
            return await PostService.PostStringAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PostDataAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            byte[] messageData)
        {
            return await PostService.PostDataAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PostValuesAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await PostService.PostValuesAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        #endregion

        #region IHttpGetService Implementation

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers)
        {
            return await GetService.GetStringAsync(logger, config, endpointUrl, headers).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            IEnumerable<Tuple<string, string>> messageData)
        {
            return await GetService.ExecuteAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            Dictionary<string, string> messageData)
        {
            return await GetService.ExecuteAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await GetService.ExecuteAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(ILogger logger, IConfigurationService config,
            Uri endpointUrl, IEnumerable<KeyValuePair<string, string>> headers)
        {
            return await GetService.ExecuteAsync(logger, config, endpointUrl, headers).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await GetService.ExecuteAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            IEnumerable<Tuple<string, string>> messageData)
        {
            return await GetService.ExecuteAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            Dictionary<string, string> messageData)
        {
            return await GetService.ExecuteAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        #endregion

        #region IHttpPutService Implementation

        public async Task<ToolsBeltHttpResponse<string>> PutJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JObject messageData)
        {
            return await _putService.PutJsonAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> PutJsonAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            JToken messageData)
        {
            return await _putService.PutJsonAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> PutStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            string messageData)
        {
            return await _putService.PutStringAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PutDataAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            byte[] messageData)
        {
            return await _putService.PutDataAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<byte[]>> PutValuesAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await _putService.PutValuesAsync(logger, config, endpointUrl, headers, messageData)
                .ConfigureAwait(false);
        }

        #endregion
    }
}