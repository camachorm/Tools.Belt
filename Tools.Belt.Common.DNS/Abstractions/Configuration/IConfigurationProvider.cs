using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Belt.Common.Abstractions.Configuration
{
    public interface IConfigurationProvider
    {
        string this[string key] { get; set; }
        string ProviderId { get; }
        IEnumerable<string> List { get; }
        IEnumerable<string> Keys { get; }
        string ReadKey(string key);
        Task<string> ReadKeyAsync(string key);
    }
}