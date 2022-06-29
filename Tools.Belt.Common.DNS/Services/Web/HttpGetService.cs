using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services.Web
{
    [ExcludeFromCodeCoverage]
    public class HttpGetService : HttpServiceBase, IHttpGetService
    {
        public HttpGetService() : this(new WebClient())
        {
        }

        public HttpGetService(WebClient client) : base(client)
        {
        }

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers)
        {
            return await GenericRequest(logger, config, headers, endpointUrl, () => Client.DownloadString(endpointUrl))
                .ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            IEnumerable<Tuple<string, string>> queryParams)
        {
            return await ExecuteAsync(logger, config, endpointUrl, headers, queryParams).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            Dictionary<string, string> queryParams)
        {
            return await ExecuteAsync(logger, config, endpointUrl, headers, queryParams).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> GetStringAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection queryParams)
        {
            return await ((IService<ToolsBeltHttpResponse<string>, Uri, IEnumerable<KeyValuePair<string, string>>,
                    NameValueCollection>) this)
                .ExecuteAsync(logger, config, endpointUrl, headers, queryParams).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(ILogger logger, IConfigurationService config,
            Uri endpointUrl, IEnumerable<KeyValuePair<string, string>> headers)
        {
            return await GetStringAsync(logger, config, endpointUrl, headers).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            IEnumerable<Tuple<string, string>> messageData)
        {
            return await GenericRequest(
                logger,
                config,
                headers,
                endpointUrl,
                () =>
                {
                    List<Tuple<string, string>> data = messageData.ToList();

                    string baseQuery = string.Empty;

                    if (string.IsNullOrEmpty(endpointUrl.Query))
                    {
                        if (data.Any()) baseQuery = "?";
                    }
                    else
                    {
                        baseQuery = endpointUrl.Query;
                        if (data.Any()) baseQuery += "&";
                    }

                    string query = data.Aggregate(baseQuery,
                        (current, item) => current + $"{item.Item1}={item.Item2}&");

                    if ((query?.EndsWith("&", StringComparison.OrdinalIgnoreCase)).GetValueOrDefault())
                        query = query?.Substring(0, query.Length - 1);

                    endpointUrl = new Uri(endpointUrl, query ?? "");


                    logger?.LogDebug(
                        $"Performing Final transformed call to {endpointUrl.AbsoluteUri}, queryParams: {endpointUrl.Query}, headers: {Client.Headers.Count}");

                    return Client.DownloadString(endpointUrl);
                }).ConfigureAwait(false);
        }

        public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            Dictionary<string, string> messageData)
        {
            return await ExecuteAsync(
                logger,
                config,
                endpointUrl,
                headers,
                messageData.Select(i => new Tuple<string, string>(i.Key, i.Value))).ConfigureAwait(false);
        }

        async Task<ToolsBeltHttpResponse<string>> IService<ToolsBeltHttpResponse<string>, Uri,
            IEnumerable<KeyValuePair<string, string>>, NameValueCollection>.ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await ExecuteAsync(
                logger,
                config,
                endpointUrl,
                headers,
                messageData.AllKeys.ToDictionary(key => key, key => messageData[key])).ConfigureAwait(false);
        }

        //public async Task<ToolsBeltHttpResponse<string>> ExecuteAsync(
        //    ILogger logger,
        //    IConfigurationService config,
        //    Uri endpointUrl,
        //    IEnumerable<KeyValuePair<string, string>> headers,
        //    string messageData)
        //{
        //    return await this.GenericRequest(logger, config, headers, endpointUrl, () => this.Client.DownloadString(endpointUrl)).ConfigureAwait(false);
        //}

        //public async Task<ToolsBeltHttpResponse<byte[]>> ExecuteAsync(
        //    ILogger logger,
        //    IConfigurationService config,
        //    Uri endpointUrl,
        //    IEnumerable<KeyValuePair<string, string>> headers,
        //    byte[] messageData)
        //{
        //    return await this.GenericRequest(logger, config, headers, endpointUrl, () => this.Client.DownloadData(endpointUrl)).ConfigureAwait(false);
        //}

        public async Task<ToolsBeltHttpResponse<byte[]>> ExecuteAsync(
            ILogger logger,
            IConfigurationService config,
            Uri endpointUrl,
            IEnumerable<KeyValuePair<string, string>> headers,
            NameValueCollection messageData)
        {
            return await GenericRequest(
                logger,
                config,
                headers,
                endpointUrl,
                () =>
                {
                    foreach (string key in messageData.AllKeys) Client.QueryString.Add(key, messageData[key]);
                    return Client.DownloadData(endpointUrl);
                }).ConfigureAwait(false);
        }
    }
}