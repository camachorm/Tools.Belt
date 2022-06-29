using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Belt.Common.Abstractions.Configuration
{
    public interface IMemoryConfigurationProvider
    {
        string this[string key] { get; set; }
        IEnumerable<string> List { get; }
        IEnumerable<string> Keys { get; }
        string ProviderId { get; }
        string ReadKey(string key);
        Task<string> ReadKeyAsync(string key);
    }
}