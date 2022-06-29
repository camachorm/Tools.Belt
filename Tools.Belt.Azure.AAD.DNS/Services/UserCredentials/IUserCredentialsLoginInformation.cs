using System;
using System.Security;

namespace Tools.Belt.Azure.AAD.Services.UserCredentials
{
    public interface IUserCredentialsLoginInformation: ILoginInformation
    {
        Func<string> GetUserName { get; }

        Func<string, SecureString> GetPassword { get; }
    }

    public class UserCredentialsLoginInformation : IUserCredentialsLoginInformation
    {
        public UserCredentialsLoginInformation(Func<string> getUserName, Func<object, SecureString> getPassword)
        {
            GetUserName = getUserName;
            GetPassword = getPassword;
        }

        public Func<string> GetUserName { get; }
        public Func<string, SecureString> GetPassword { get; }
    }
}