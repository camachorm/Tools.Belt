using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.ServiceBus.DNS.Messages.Extensions
{
    public static class MessageEnvelopeExtensions
    {
        public static BlobAttachmentMessageEnvelope ToBlobAttachmentMessageEnvelope(this MessageEnvelope message)
        {
            return new BlobAttachmentMessageEnvelope(message.OriginalMessage);
        }

        public static async Task<byte[]> GetMessageBody(this Message source, string blobConnectionString)
        {
            return source.UserProperties.ContainsKey(ServiceBusConfigurationProperties.AttachmentBlobProperty)
                ? (await source.DownloadAzureStorageAttachment(
                    new AzureStorageAttachmentConfiguration(blobConnectionString)))?.Body
                : source.Body;
        }

        public static async Task<T> GetMessageBody<T>(this Message source, string blobConnectionString)
        {
            return DeserializeFromByteArray<T>(await source.GetMessageBody(blobConnectionString).ConfigureAwait(false));
        }

        /// <summary>
        /// Delays message processing on the executing server when a failed message arrives.
        /// </summary>
        /// <param name="messages">Messages being processed.</param>
        /// <param name="messageReceiver">Message receiver.</param>
        /// <param name="delayPerFailedDeliveryMs">The maximum failed delivery from all messages is multiplied by this value. Milliseconds.</param>
        /// <returns>Task to await on.</returns>
        /// <remarks>
        /// Make sure the lock token on your messages lasts long enough to allow for a maximum of
        /// 10 * <paramref name="delayPerFailedDeliveryMs"/>, or the delay function will either not work properly or fail.
        /// </remarks>
        public static async Task Throttle(this Message[] messages, MessageReceiver messageReceiver, int delayPerFailedDeliveryMs = 2500)
        {
            // Increasing delay on failure to remedy throttling.
            int failedDeliveryCount = messages.Max(x => x.SystemProperties.DeliveryCount) - 1;
            if (failedDeliveryCount == 0) return;

            int delayMs = failedDeliveryCount * delayPerFailedDeliveryMs;

            int lockLeftMs = (int)messages.Min(m => m.SystemProperties.LockedUntilUtc - DateTime.UtcNow).TotalMilliseconds;
            lockLeftMs -= 5000; // Subtracting 5 seconds to accomodate the action of renewing the tokens.

            if (delayMs > lockLeftMs)
            {
                await messages.AsParallel().ForAllAsync(m => messageReceiver.RenewLockAsync(m.SystemProperties.LockToken), messages.Length)
                    .ConfigureAwait(false);
                lockLeftMs = (int)messages.Min(m => m.SystemProperties.LockedUntilUtc - DateTime.UtcNow).TotalMilliseconds;
                lockLeftMs -= 5000; // Subtracting 5 seconds to accomodate the action of renewing the tokens.
                if (delayMs > lockLeftMs) // If the lock time is still not enough, adjust the delay.
                {
                    delayMs = lockLeftMs;
                }
            }

            await Task.Delay(delayMs).ConfigureAwait(false);
            await messages.AsParallel().ForAllAsync(m => messageReceiver.RenewLockAsync(m.SystemProperties.LockToken), messages.Length)
                .ConfigureAwait(false);
        }

        private static T DeserializeFromByteArray<T>(byte[] source)
        {
            MemoryStream stream = new MemoryStream(source);
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);

            T o = (T) formatter.Deserialize(stream);
            return o;
        }
    }
}