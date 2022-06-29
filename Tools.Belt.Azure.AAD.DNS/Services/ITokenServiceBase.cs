using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Tools.Belt.Azure.AAD.Services
{
    public interface ITokenServiceBase
    {
        /// <summary>
        /// The ClientId of the running application in AAD.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Contains the list of scopes to be requested during authentication.
        /// </summary>
        IEnumerable<string> Scopes { get; }

        /// <summary>
        /// Contains the <see cref="IClientApplicationBase"/> in use.
        /// </summary>
        IClientApplicationBase ApplicationBase { get; }

        /// <summary>
        /// Returns the <see cref="Microsoft.Identity.Client.AuthenticationResult"/> instance returned from the authentication operation carried out by <see cref="Login"/> or <see cref="LoginAsync"/>.
        /// </summary>
        AuthenticationResult AuthenticationResult { get; }

        /// <summary>
        /// Returns true if the authentication process has been carried out successfully, false otherwise.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Method used to login against AAD
        /// </summary>
        void Login();

        /// <summary>
        /// Asynchronous method used to login against AAD
        /// </summary>
        Task LoginAsync();
    }
}