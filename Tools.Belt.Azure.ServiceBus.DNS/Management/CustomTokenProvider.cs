using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.Services.AppAuthentication;

namespace Tools.Belt.Azure.ServiceBus.DNS.Management
{
    public class CustomTokenProvider : AzureServiceTokenProvider, Microsoft.Rest.ITokenProvider, Microsoft.Azure.ServiceBus.Primitives.ITokenProvider
    {

        public CustomTokenProvider(string resource, string tenantId)
        {
            Resource = resource;
            TenantId = tenantId;
        }
        public async Task<SecurityToken> GetTokenAsync(string appliesTo, TimeSpan timeout)
        {
            return new JsonSecurityToken(await GetAccessTokenAsync(Resource, TenantId), Resource);
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            return new AuthenticationHeaderValue("Bearer", await  GetAccessTokenAsync(Resource, TenantId));
        }

        public string TenantId { get; }
        public string Resource { get; }
    }
}