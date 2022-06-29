using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services.Web
{
    public interface IHttpServiceBase
    {
        Task<ToolsBeltHttpResponse<TOut>> GenericRequest<TOut>(
            ILogger logger,
            IConfigurationService config,
            IEnumerable<KeyValuePair<string, string>> headers,
            Uri endpointUri,
            Func<TOut> executionFunc);

        ToolsBeltHttpResponse<TOut> CarryOutRequest<TOut>(
            ILogger logger,
            IConfigurationService config,
            IEnumerable<KeyValuePair<string, string>> headers,
            Uri endpointUri,
            Func<TOut> executionFunc);
    }
}