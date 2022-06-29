using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Core;
using Tools.Belt.Azure.ServiceBus.DNS.Messages;

namespace Tools.Belt.Azure.ServiceBus.DNS.Receiver
{
    public interface IQueueReceiver
    {
        Tuple<bool, ICollection<string>> GetAllMessages(IMessageReceiver messageReceiver);

        IEnumerable<MessageEnvelope> GetAllMessagesWithProperties(IMessageReceiver messageReceiver,
            int threadSleep = 1000);

        IMessageReceiver RegisterSbPlugin(string topicName, string subscriptionName, string sbConnString,
            string blobConnString, string blobStorageName);

        Task UnRegisterSbPlugin(IMessageReceiver receiver);
    }
}