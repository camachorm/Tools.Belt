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

namespace Tools.Belt.Azure.ServiceBus.DNS.Sender
{
    [ExcludeFromCodeCoverage]
    public abstract class QueueSenderBase<T> : IQueueSender<T> where T : class
    {
        private const string ServiceContentType = "application/xml";

        // messages with body > 200KB should be converted to use attachments
        private const int MaximumMessageSize = 200 * 1024;
        private readonly (ILogger Logger, TelemetryClient telemetryClient) _ctor;
        private string _queueName;

        protected QueueSenderBase(ILogger logger, TelemetryClient telemetryClient)
        {
            _ctor = (logger, telemetryClient);
        }

        private Func<Message, bool> MessageSizeCriteria => message => message.Body.Length > MaximumMessageSize;

        public Tuple<IMessageSender, string> RegisterSbPlugin(string topicName, string subscriptionName,
            string sbConnString, string blobConnString, string blobStorageName)
        {
            Stopwatch sw = Stopwatch.StartNew();
            MessageSender responseSender = new MessageSender(sbConnString, topicName);
            AzureStorageAttachmentConfiguration config =
                new AzureStorageAttachmentConfiguration(blobConnString,
                    messageMaxSizeReachedCriteria: MessageSizeCriteria);
            string pluginName = responseSender.RegisterAzureStorageAttachmentPlugin(config).Name;
            _ctor.Logger.LogDebug($"Registered ServiceBus Storage Plugin in {sw.ElapsedMilliseconds:n0}ms",
                _ctor.telemetryClient);
            return new Tuple<IMessageSender, string>(responseSender, pluginName);
        }

        public Tuple<IMessageSender, string> RegisterSbPlugin(string queueName, string sbConnString,
            string blobConnString, string blobStorageName)
        {
            _queueName = queueName;
            Stopwatch sw = Stopwatch.StartNew();
            MessageSender responseSender = new MessageSender(sbConnString, queueName);

            AzureStorageAttachmentConfiguration config =
                new AzureStorageAttachmentConfiguration(blobConnString,
                    messageMaxSizeReachedCriteria: MessageSizeCriteria);
            string pluginName = responseSender.RegisterAzureStorageAttachmentPlugin(config).Name;
            _ctor.Logger.LogDebug($"Registered ServiceBus Storage Plugin in {sw.ElapsedMilliseconds:n0}ms",
                _ctor.telemetryClient);
            return new Tuple<IMessageSender, string>(responseSender, pluginName);
        }

        public void UnRegisterSbPlugin(IMessageSender sender, string pluginName)
        {
            Stopwatch sw = Stopwatch.StartNew();
            sender.UnregisterPlugin(pluginName);
            _ctor.Logger.LogDebug(
                $"UnRegistered ServiceBus Storage Plugin at Receiver end in {sw.ElapsedMilliseconds:n0}ms",
                _ctor.telemetryClient);
        }

        public async Task<IEnumerable<Guid>> SendMessages(IEnumerable<T> requestMessage, IMessageSender messageSender)
        {
            List<Guid> result = new List<Guid>();

            foreach (T message in requestMessage)
            {
                Tuple<bool, Guid> tuple = await SendMessage(message, messageSender, Guid.NewGuid());
                if (tuple.Item1)
                    result.Add(tuple.Item2);
            }

            return result;
        }

        public async Task<Tuple<bool, Guid>> SendMessage(T requestMessage, IMessageSender messageSender, Guid messageId)
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool returnVal = false;
            byte[] payloadAsBytes = Encoding.UTF8.GetBytes(requestMessage.ToString());
            Message message = new Message(payloadAsBytes)
            {
                MessageId = messageId.ToString(),
                ContentType = ServiceContentType,
                TimeToLive = TimeSpan.MaxValue
            };
            try
            {
                await messageSender.SendAsync(message).ContinueWith(task => ContinuationFunction(task, sw,
                    out returnVal), TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (Exception e)
            {
                _ctor.Logger.LogError(
                    $"Error receiving message from Azure Service Bus/Blob storage:\nError Message : {e.Message}\nStack Trace : {e.StackTrace}\nSource : {e.Source}");
            }

            return new Tuple<bool, Guid>(returnVal, messageId);
        }

        public async Task SendMessagesBatched(IEnumerable<Message> messages, IMessageSender messageSender)
        {
            const int maxDegreeOfParallelism = 20;
            SemaphoreSlim throttler = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);
            Stopwatch stopwatchOperation = Stopwatch.StartNew();
            List<Message> currentBatch = new List<Message>();
            long currentBatchSize = 0;
            int counter = 0;
            int counterMessages = 0;
            List<Task> taskList = new List<Task>();

            Stopwatch stopwatchEnqueue = Stopwatch.StartNew();
            _ctor.Logger.LogDebug("Enqueuing messages in batches...");
            foreach (Message message in messages)
            {
                counterMessages++;
                currentBatchSize += message.Size;
                currentBatch.Add(message);
                if (currentBatchSize + message.Size >= MaximumMessageSize)
                {
                    await throttler.WaitAsync();
                    counter++;
                    // Since the task is created with a lambda function, the message list has to be created
                    // beforehand and not as part of the lambda function, as it will treat it by reference
                    // until passed (and thus pass a cleared one as the list is cleared right after).
                    List<Message> messageList = new List<Message>(currentBatch);
                    Task<bool> task = Task.Run(async () =>
                        await SendMessageList(messageList, messageSender, throttler));
                    taskList.Add(task);
                    currentBatch.Clear();
                    currentBatchSize = 0;
                }
            }

            if (currentBatch.Any())
            {
                await throttler.WaitAsync();
                counter++;
                // Since the task is created with a lambda function, the message list has to be created
                // beforehand and not as part of the lambda function, as it will treat it by reference
                // until passed (and thus pass a cleared one as the list is cleared right after).
                List<Message> messageList = new List<Message>(currentBatch);
                Task<bool> task = Task.Run(async () => await SendMessageList(messageList, messageSender, throttler));
                taskList.Add(task);
                currentBatch.Clear();
                currentBatchSize = 0;
            }

            await Task.WhenAll(taskList);

            throttler.Dispose();
            _ctor.Logger.LogDebug(
                $"Enqueued {++counter} batches with {counterMessages} messages to ServiceBus in {stopwatchEnqueue.ElapsedMilliseconds}ms.");
        }

        public async Task<bool> SendMessageList(IList<Message> messages, IMessageSender sender, SemaphoreSlim throttler)
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool returnVal = false;

            try
            {
                Task task = sender.SendAsync(messages);
                await task.ConfigureAwait(false);
                ContinuationFunction(task, sw, out returnVal);
            }
            catch (Exception e)
            {
                _ctor.Logger.LogError(
                    $"Error receiving message from Azure Service Bus/Blob storage:\nError Message : {e.Message}\nStack Trace : {e.StackTrace}\nSource : {e.Source}");
            }
            finally
            {
                throttler.Release();
            }

            return returnVal;
        }

        private void ContinuationFunction(Task t, Stopwatch stopwatch, out bool returnVal)
        {
            if (t.IsCompleted && !(t.IsFaulted || t.IsCanceled))
            {
                _ctor.Logger.LogDebug(
                    $"Successfully sent message to queue '{_queueName}'  using ServiceBus Storage Plugin in {stopwatch.ElapsedMilliseconds}ms");
                returnVal = true;
            }
            else
            {
                _ctor.Logger.LogError(
                    $"Error sending message to queue '{_queueName}' on Azure Service Bus/Blob storage. ",
                    _ctor.telemetryClient,
                    t.Exception);

                returnVal = false;
            }
        }
    }
}