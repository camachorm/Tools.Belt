using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.ServiceBus.DNS.Extensions
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// Sends all the <paramref name="objects"/> provided in 1 or multiple batches limited by the sum of <see cref="Message.Size"/>
        /// being smaller than <paramref name="maximumBatchSize"/>.
        /// </summary>
        /// <param name="sender">The <see cref="IMessageSender"/> instance used to send the messages.</param>
        /// <param name="objects">The list of objects to send.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="maximumBatchSize">The maximum size in bytes supported by Service bus for a single batched send operation. 
        /// e.g. 256 KB for Standard tier, 1 MB for Premium tier. Minimum 64 KB.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="Task"/> associated with all the </returns>
        public static async Task SendBatchedJsonAsync(
            this IMessageSender sender,
            IEnumerable<object> objects,
            ILogger logger,
            long maximumBatchSize = 256000,
            int? maxDegreeOfParallelism = null)
        {
            IEnumerable<Message> msgs = objects.Select(m => m.ToJsonOldServiceBusMessage());
            await sender.SendBatchedAsync(msgs, logger, maximumBatchSize, maxDegreeOfParallelism).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends all the <paramref name="messages"/> provided in 1 or multiple batches limited by the sum of <see cref="Message.Size"/>
        /// being smaller than <paramref name="maximumBatchSize"/>.
        /// </summary>
        /// <param name="sender">The <see cref="IMessageSender"/> instance used to send the <see cref="messages"/> list.</param>
        /// <param name="messages">The list of <see cref="Message"/> to send.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="maximumBatchSize">The maximum size in bytes supported by Service bus for a single batched send operation. 
        /// e.g. 256 KB for Standard tier, 1 MB for Premium tier. Minimum 64 KB.</param>
        /// <returns>A <see cref="IList{T}"/> of <see cref="Task"/> associated with all the </returns>
        public static async Task SendBatchedAsync(
            this IMessageSender sender,
            IEnumerable<Message> messages,
            ILogger logger,
            long maximumBatchSize = 256000,
            int? maxDegreeOfParallelism = null)
        {
            const int headerSize = 64000;
            if (maximumBatchSize <= headerSize) throw new ArgumentException($"Must be bigger than {headerSize}.", nameof(maximumBatchSize));

            // There is a 256 KB/1Mb limit per message sent on Azure Service Bus.
            // We will divide it into messages block lower or equal to 256 KB/1Mb.
            // Maximum message size: 256 KB for Standard tier, 1 MB for Premium tier.
            // Maximum header size: 64 KB.
            // https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quotas
            var maxBatchSize = maximumBatchSize - headerSize;
            
            await messages.SplitIntoBatches(maxBatchSize).AsParallel().ForAllAsync(async batch =>
            {
                try
                {
                    await sender.SendAsync(batch);
                }
                catch (MessageSizeExceededException e) when (e.Message.Contains($"Unable to batch.") == false)
                {
                    // Due to system overhead, this limit is less than these values.
                    // https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quotas
                    await sender.SendAsync(batch.Take(batch.Count / 2).ToList());
                    await sender.SendAsync(batch.Skip(batch.Count / 2).ToList());
                    logger.LogWarning(e, "Oversized service bus batch sent in two parts. " +
                        $"This is not a transient slowdown, please reduce '{nameof(maximumBatchSize)}'.");
                }
            }, maxDegreeOfParallelism: maxDegreeOfParallelism).ConfigureAwait(false);
        }

        private static IEnumerable<IList<Message>> SplitIntoBatches(
            this IEnumerable<Message> messages,
            long maxBatchSize)
        {
            var pageSize = 0L;
            var pagedMessages = new List<Message>();
            var enumerator = messages.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null) continue;
                if (pageSize + enumerator.Current.Size > maxBatchSize)
                {
                    if (pagedMessages.Any() == false) 
                        throw new MessageSizeExceededException($"Unable to batch. Message of size '{enumerator.Current.Size}'" +
                            $" exceeds the max '{maxBatchSize}'.");
                    yield return pagedMessages;
                    pageSize = 0L;
                    pagedMessages = new List<Message>();
                }

                pageSize += enumerator.Current.Size;
                pagedMessages.Add(enumerator.Current);
            }
            yield return pagedMessages;
            enumerator.Dispose();
        }

        public static async Task SendBatchAsync(this IMessageSender sender, IEnumerable<Message> messages, long maximumBatchSize = 256000)
        {
            var sendMessagesId = 0;
            var sendMessagesBlock = new ActionBlock<List<Message>>(async list =>
            {
                var id = ++sendMessagesId;
                try
                {
                    foreach (var messageBatch in messages.SplitBySize(maximumBatchSize))
                    {
                        var sum = messageBatch.Sum(message => message.Size);
                        await sender.SendAsync(messageBatch);
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (MessageSizeExceededException)
                {
                    // Due to system overhead, this limit is less than these values.
                    // https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quotas
                    maximumBatchSize = (long)(maximumBatchSize * 0.9);
                    var index = list.Count / 2;
                    var batch1 = list.Take(index).ToList();
                    await sender.SendAsync(batch1);
                    list.RemoveRange(0, index);
                    await sender.SendAsync(list);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            });

            var iterateMessagesBlock = new TransformManyBlock<SplitMessageArgs, List<Message>>(args =>
            {
                var messagesList = args.Messages.ToList();

                List<List<Message>> result = new List<List<Message>>();
                var avgSize = args.Messages.Average(message => message.Size);
                var maxMessages = (int)Math.Round(maximumBatchSize / avgSize, 0);

                while (messagesList.Any())
                {
                    var items = messagesList.Take(maxMessages).ToList();
                    result.Add(items);
                    messagesList.RemoveRange(0, items.Count);
                }

                return result;

            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
            });
            iterateMessagesBlock.LinkTo(sendMessagesBlock, new DataflowLinkOptions { PropagateCompletion = true });
            iterateMessagesBlock.Post(new SplitMessageArgs()
            {
                MaxBatchSize = maximumBatchSize,
                Messages = messages
            });

            iterateMessagesBlock.Complete();
            await sendMessagesBlock.Completion;
        }

        private struct SplitMessageArgs
        {
            public IEnumerable<Message> Messages { get; set; }
            public long MaxBatchSize { get; set; }
        }
    }
}
