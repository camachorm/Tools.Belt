using System;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.AAD.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidUserName(this string userName, string domainName = null)
        {
            if (userName.IsNullOrEmpty() || userName.IsNullOrWhiteSpace())
            {
                return false;
            }

            // ReSharper disable AssignNullToNotNullAttribute
            if (!domainName.IsNullOrWhiteSpace() && !userName.EndsWith(domainName, StringComparison.InvariantCultureIgnoreCase) && userName.Contains("@"))
            // ReSharper restore AssignNullToNotNullAttribute
            {
                return false;
            }

            if (!userName.Contains("@")) return false;

            return true;
        }

        // ReSharper disable MemberCanBePrivate.Global
        public static string EnsureValidDomainName(this string domainName)
        // ReSharper restore MemberCanBePrivate.Global
        {
            if (!domainName.StartsWith("@"))
                domainName = $"@{domainName}";
            if (!domainName.Contains("."))
                throw new ArgumentException("Domain name must be in the format of @domain.extension e.g. - @tools.belt or @google.com", nameof(domainName));
            return domainName;
        }

        public static string EnsureUserNameHasDomainName(this string userName, string domainName = null)
        {
            if (!domainName.IsNullOrWhiteSpace() && !userName.EndsWith(domainName.EnsureValidDomainName(), StringComparison.InvariantCultureIgnoreCase))
                userName += domainName.EnsureValidDomainName();
            return userName;
        }
    }
}
