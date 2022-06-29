using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.ServiceBus;

namespace Tools.Belt.Azure.ServiceBus.DNS.Messages
{
    [ExcludeFromCodeCoverage]
    public class MessageEnvelope
    {
        public string Message { get; set; }
        public Message OriginalMessage { get; set; }
        public IDictionary<string, object> UserProperties { get; set; }
    }
}