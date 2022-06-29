using System;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.ServiceBus.DNS.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts the object into a service bus message of type json, where the body represents the object.
        /// </summary>
        /// <param name="source">Object to convert.</param>
        /// <returns>Service bus message.</returns>
        [Obsolete("This method was used with a prior version of the ServiceBus Library, change to ToJsonServiceBusMessage() instead.")]
        public static Message ToJsonOldServiceBusMessage(this object source)
        {
            Message msg = new Message()
            {
                ContentType = "application/json",
                Body = source.ToJsonByteArray()
            };

            return msg;
        }

         /// <summary>
        /// Converts the object into a service bus message of type json, where the body represents the object.
        /// </summary>
        /// <param name="source">Object to convert.</param>
        /// <param name="partitionKey">[Optional] - The partition key to be used in the message</param>
        /// <param name="messageId">[Optional] - The message Id to be used in the message</param>
        /// <returns>Service bus message.</returns>
        public static ServiceBusMessage ToJsonServiceBusMessage(this object source, string partitionKey = null, string messageId = null, int messageIterationCount = 1)
        {
            ServiceBusMessage msg = new ServiceBusMessage()
            {
                ContentType = "application/json",
                Body = new BinaryData(source.ToJsonByteArray()),
            };
            if (messageId != null)
                msg.MessageId = messageId;
            if (partitionKey != null)
                msg.PartitionKey = partitionKey;
            
            msg.ApplicationProperties.Add("IterationCount", messageIterationCount);

            return msg;
        }

    }
}
