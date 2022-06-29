using System.Collections.Generic;

namespace Tools.Belt.Common.Abstractions.Configuration
{
    public interface IConfigurationService
    {
        string this[string key] { get; set; }
        IEnumerable<string> List { get; }
        IEnumerable<string> Keys { get; }
        IEnumerable<string> ProviderList { get; }
        IConfigurationService RegisterProvider(IConfigurationBuilder builder, IConfigurationProvider newProvider);
    }
}