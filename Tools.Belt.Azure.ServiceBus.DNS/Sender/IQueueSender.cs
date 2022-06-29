// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Core;

namespace Tools.Belt.Azure.ServiceBus.DNS.Sender
{
    public interface IQueueSender<in T> where T : class
    {
        Task<Tuple<bool, Guid>> SendMessage(T t, IMessageSender messageSender, Guid messageId);

        Task<IEnumerable<Guid>> SendMessages(IEnumerable<T> requestMessage, IMessageSender messageSender);

        Tuple<IMessageSender, string> RegisterSbPlugin(string topicName, string subscriptionName, string sbConnString,
            string blobConnString, string blobStorageName);

        Tuple<IMessageSender, string> RegisterSbPlugin(string queueName, string sbConnString, string blobConnString,
            string blobStorageName);

        void UnRegisterSbPlugin(IMessageSender sender, string pluginName);
    }
}