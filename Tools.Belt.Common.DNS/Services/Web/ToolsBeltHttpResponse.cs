using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Tools.Belt.Common.Services.Web
{
    [ExcludeFromCodeCoverage]
    public class ToolsBeltHttpResponse<T>
    {
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
            Justification = "POCO class requires the collection to be injected from source.")]
        public WebHeaderCollection ResponseHeaders { get; set; }

        public Uri RequestUri { get; set; }

        public T ResponseValue { get; set; }
    }
}