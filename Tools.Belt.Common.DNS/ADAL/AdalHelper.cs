using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.ADAL
{
    // We exclude this because there is no possibility of mocking, it requires http connectivity to an actual AD server
    [ExcludeFromCodeCoverage]
    public static class AdalHelper
    {
        public static async Task<string> GetTokenAsync(
            string clientId,
            string clientSecret,
            string aadInstance,
            string tenantId,
            string organizationHostName)
        {
            string loginUrl = aadInstance.AddUriSegment(tenantId).AddUriSegment("oauth2").AddUriSegment("token");
            string resource = organizationHostName;

            using HttpClient client = new HttpClient();
            string postData =
                $"client_id={clientId}&client_secret={clientSecret}&resource={resource}&grant_type=client_credentials";

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl)
            {
                Content = new StringContent(postData, Encoding.UTF8)
            };

            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpResponseMessage responseMessage = await client.SendAsync(request).ConfigureAwait(false);
            string jsonResponseString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            Dictionary<string, string> jsonContent =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponseString);

            return jsonContent["access_token"];
        }

        public static string GetToken(
            string clientId,
            string clientSecret,
            string aadInstance,
            string tenantId,
            string organizationHostName)
        {
            Task<string> t = GetTokenAsync(clientId, clientSecret, aadInstance, tenantId, organizationHostName);
            t.Wait();
            return t.Result;
        }
    }
}