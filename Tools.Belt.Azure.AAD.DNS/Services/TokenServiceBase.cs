using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Tools.Belt.Azure.AAD.Services
{
    public abstract class TokenServiceBase : ITokenServiceBase
    {
        protected readonly AzureCloudInstance AzureCloudInstance;
        protected readonly AadAuthorityAudience AadAuthorityAudience;

        protected TokenServiceBase(
            string clientId, 
            IEnumerable<string> scopes, 
            AzureCloudInstance azureCloudInstance, 
            AadAuthorityAudience aadAuthorityAudience,
            ILogger logger,
            string tenantId)
        {
            AzureCloudInstance = azureCloudInstance;
            AadAuthorityAudience = aadAuthorityAudience;
            ClientId = clientId;
            TenantId = tenantId;
            Scopes = scopes;
            Logger = logger;
            // ReSharper disable VirtualMemberCallInConstructor
            InitializeClientApplication();
            // ReSharper restore VirtualMemberCallInConstructor
        }
        
        // ReSharper disable MemberCanBeProtected.Global
        /// <inheritdoc/>
        public string ClientId { get; }
        // ReSharper disable MemberCanBePrivate.Global

        public string TenantId { get; }

        /// <inheritdoc/>
        public IEnumerable<string> Scopes { get; }

        protected ILogger Logger { get; }
        // ReSharper restore MemberCanBePrivate.Global

        /// <inheritdoc/>
        public IClientApplicationBase ApplicationBase { get; protected set; }
        
        /// <inheritdoc/>
        public AuthenticationResult AuthenticationResult { get; protected set; }

        /// <inheritdoc/>
        public bool IsAuthenticated => AuthenticationResult != null;

        /// <inheritdoc/>
        public abstract void Login();

        /// <inheritdoc/>
        public abstract Task LoginAsync();
        // ReSharper restore MemberCanBeProtected.Global

        /// <summary>
        /// Method that when implemented in a derived class will perform the initialization of <see cref="ApplicationBase"/>
        /// </summary>
        protected abstract void InitializeClientApplication();
    }
}
