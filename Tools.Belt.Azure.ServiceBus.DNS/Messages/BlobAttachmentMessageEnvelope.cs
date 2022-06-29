using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Azure.ServiceBus;

namespace Tools.Belt.Azure.ServiceBus.DNS.Messages
{
    [ExcludeFromCodeCoverage]
    public class BlobAttachmentMessageEnvelope
    {
        public BlobAttachmentMessageEnvelope(MessageEnvelope envelope) : this(envelope?.OriginalMessage)
        {
        }

        public BlobAttachmentMessageEnvelope(Message source)
        {
            Source = source;
        }

        public Message Source { get; }

        public string Message => Encoding.Default.GetString(Source.Body);
        public IDictionary<string, object> UserProperties => Source.UserProperties;
        public string BlobId => UserProperties[ServiceBusConfigurationProperties.AttachmentBlobProperty]?.ToString();

        public string SequenceNumber =>
            UserProperties[ServiceBusConfigurationProperties.SequenceNumberProperty]?.ToString();
    }
}