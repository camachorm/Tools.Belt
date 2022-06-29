using System.Diagnostics.CodeAnalysis;

namespace Tools.Belt.Azure.ServiceBus.DNS.Messages
{
    [ExcludeFromCodeCoverage]
    public static class ServiceBusConfigurationProperties
    {
        public const string AttachmentBlobProperty = "$attachment.blob";
        public const string SequenceNumberProperty = "x-opt-enqueue-sequence-number";
    }
}