using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Tools.Belt.Azure.ServiceBus.DNS.Extensions
{
    public static class MessageExtensions
    {
        /// <summary>
        /// After some research we assessed that we need a minimum buffer of 64.000 bytes for the <see cref="Message"/>
        /// envelope metadata, and another 10.000 for system overhead in edge cases.
        /// There is a 256 KB/1Mb limit per message sent on Azure Service Bus.   
        /// We will divide it into messages block lower or equal to 256 KB/1Mb.
        /// Maximum message size: 256 KB for Standard tier, 1 MB for Premium tier.
        /// Maximum header size: 64 KB.
        /// https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quotas
        /// </summary>
        public const int MessageEnvelopeSize = 74000;

        public static IEnumerable<IList<Message>> SplitBySize(this IEnumerable<Message> source, long maximumBatchSize)
        {
            var batch = new List<Message>();
            var enumerator = source.GetEnumerator();
            // ReSharper disable PossibleMultipleEnumeration
            while (enumerator.MoveNext())
            {
                if (batch.Sum(message => message.MessageSizeUpperLimit()) + enumerator.Current?.MessageSizeUpperLimit() > maximumBatchSize)
                {
                    var result = batch.ToList();
                    batch.Clear();
                    yield return result;
                }
                batch.Add(enumerator.Current);
            }
            yield return batch;
            // ReSharper restore PossibleMultipleEnumeration
            enumerator.Dispose();
        }

        /// <summary>
        /// Returns the maximum expected size for this <see cref="Message"/>, which is the sum between
        /// <see cref="Message.Size"/> and <see cref="MessageEnvelopeSize"/>
        /// </summary>
        /// <param name="source">The source <see cref="Message"/>.</param>
        /// <returns>The maximum expected size for the message after it is sent to ServiceBus.</returns>
        public static long MessageSizeUpperLimit(this Message source)
        {
            return source.Size + MessageEnvelopeSize;
        }

        /// <summary>
        ///  Exponentially await in increments of ms (50 by default) for each delivery attempt to attempt recovering
        ///  from transient errors as experienced during early trials.
        /// </summary>
        /// <param name="sbMessage">Message.</param>
        /// <param name="increment">Increment in milliseconds.</param>
        /// <returns><see cref="Task" /></returns>
        public static async Task TransientErrorRecoveryDelayAsync(this Message sbMessage, int increment = 50)
        {
            if (sbMessage == null) return;

            await TransientErrorRecoveryDelayAsync(sbMessage.SystemProperties.DeliveryCount, increment);
        }

        /// <summary>
        /// Exponentially await in increments of ms (50 by default) for each delivery attempt to attempt recovering
        /// from transient errors as experienced during early trials.
        /// </summary>
        /// <param name="deliveryCount">Message delivery count.</param>
        /// <param name="increment">Increment in milliseconds.</param>
        /// <returns><see cref="Task" /></returns>
        public static async Task TransientErrorRecoveryDelayAsync(int deliveryCount, int increment = 50)
        {
            await Task.Delay(TimeSpan.FromMilliseconds((deliveryCount - 1) * increment));
        }
    }
}
