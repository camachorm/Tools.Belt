using Microsoft.Identity.Client;

namespace Tools.Belt.Azure.AAD.Services.UserCredentials
{
    public interface IUserCredentialsTokenService: ITokenServiceBase
    {
        IPublicClientApplication ClientApplication { get; }
    }
}