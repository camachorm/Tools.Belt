// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Tools.Belt.Azure.ServiceBus.DNS.Messages;

namespace Tools.Belt.Azure.ServiceBus.DNS.Receiver
{
    [ExcludeFromCodeCoverage]
    public abstract class QueueReceiverBase : IQueueReceiver
    {
        protected readonly (ILogger Logger, TelemetryClient telemetryClient) Ctor;

        protected QueueReceiverBase(ILogger logger, TelemetryClient telemetryClient)
        {
            Ctor = (logger, telemetryClient);
        }

        public IMessageReceiver RegisterSbPlugin(string topicName, string subscriptionName, string sbConnString,
            string blobConnString, string blobStorageName)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string topicFullName = EntityNameHelper.FormatSubscriptionPath(topicName, subscriptionName);
            MessageReceiver receiver = new MessageReceiver(sbConnString, topicFullName);
            AzureStorageAttachmentConfiguration config =
                new AzureStorageAttachmentConfiguration(blobConnString, blobStorageName);
            receiver.RegisterAzureStorageAttachmentPlugin(config);
            Ctor.Logger.LogDebug(
                $"Registered ServiceBus Storage Plugin at Receiver end in {sw.ElapsedMilliseconds:n0}ms");
            return receiver;
        }

        public async Task UnRegisterSbPlugin(IMessageReceiver receiver)
        {
            Stopwatch sw = Stopwatch.StartNew();
            await receiver.CloseAsync();
            Ctor.Logger.LogDebug(
                $"UnRegistered ServiceBus Storage Plugin at Receiver end in {sw.ElapsedMilliseconds:n0}ms");
        }

        public Tuple<bool, ICollection<string>> GetAllMessages(IMessageReceiver messageReceiver)
        {
            List<MessageEnvelope> result = GetAllMessagesWithProperties(messageReceiver).ToList();
            return new Tuple<bool, ICollection<string>>(true, result.Select(item => item.Message).ToList());
        }

        public IEnumerable<MessageEnvelope> GetAllMessagesWithProperties(IMessageReceiver messageReceiver,
            int threadSleep = 1000)
        {
            Stopwatch sw = Stopwatch.StartNew();

            ServiceBusPlugin sbPlugin = null;
            IList<MessageEnvelope> topicMessages = new List<MessageEnvelope>();

            if (messageReceiver.RegisteredPlugins.Count > 0)
                sbPlugin = messageReceiver.RegisteredPlugins.SingleOrDefault(plugin =>
                    !string.IsNullOrEmpty(plugin.Name));

            try
            {
                Task OnMessageHandlerExceptions(ExceptionReceivedEventArgs e)
                {
                    Ctor.Logger.LogError(
                        "OnMessageHandlerExceptions: Error receiving message from Azure Service Bus/Blob storage. ",
                        Ctor.telemetryClient, e.Exception);

                    return Task.CompletedTask;
                }

                MessageHandlerOptions messageHandlerOptions = new MessageHandlerOptions(OnMessageHandlerExceptions)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false,
                    MaxAutoRenewDuration = TimeSpan.FromMilliseconds(300001)
                };

                if (sbPlugin != null)
                    messageReceiver.RegisterMessageHandler(
                        async (message, token) =>
                        {
                            if (message.Body != null)
                                topicMessages.Add(
                                    new MessageEnvelope
                                    {
                                        OriginalMessage = message,
                                        Message = Encoding.Default.GetString(message.Body),
                                        UserProperties = message.UserProperties
                                    });

                            await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
                        },
                        messageHandlerOptions);

                Thread.Sleep(threadSleep);
                Ctor.Logger.LogDebug(
                    $"Successfully received messages using ServiceBus Storage Plugin in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception e)
            {
                Ctor.Logger.LogError($"Error Message : {e.Message}\nStack Trace : {e.StackTrace}\nSource : {e.Source}",
                    Ctor.telemetryClient, e);
            }

            return topicMessages;
        }
    }
}