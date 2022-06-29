using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Tools.Belt.Azure.ServiceBus.DNS
{
    [DebuggerStepThrough]
    [ExcludeFromCodeCoverage]
    public abstract class SbPluginConfigProvider
    {
        public readonly string BlobConnString;
        public readonly string BlobStorageName;

        public readonly string SbConnString;
        public readonly string SubscriptionName;
        public readonly int ThreadSleep;
        public readonly string TopicName;

        protected SbPluginConfigProvider(string sbConnString, string topicName, string blobConnString,
            string blobStorageName, string subscriptionName, int threadSleep)
        {
            SbConnString = sbConnString;
            TopicName = topicName;
            BlobConnString = blobConnString;
            BlobStorageName = blobStorageName;
            SubscriptionName = subscriptionName;
            ThreadSleep = threadSleep;
        }
    }
}