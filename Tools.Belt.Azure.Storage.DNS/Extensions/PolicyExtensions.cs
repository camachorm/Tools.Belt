using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Logging;
using Polly;
using Tools.Belt.Azure.Storage.DNS.Common;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.Storage.DNS.Extensions
{
    public static class PolicyExtensions
    {
        public static async Task<T> HandleWaitAndRetryStorageOperationAsync<T>(this IStorageService source,
            IConfigurationService configuration, ILogger logger, string retryMessage, Func<Context, Task<T>> asyncFunc)
        {
            return await Policy.Handle<StorageException>().WaitAndRetryAsync(
                    configuration["MaximumRetries"].SafeToInt(5),
                    (i, context) =>
                        TimeSpan.FromMilliseconds(Math.Pow(configuration["LeaseRetryInterval"].SafeToInt(50), i)),
                    (exception, span, arg3, arg4) =>
                        logger.LogDebug(string.Format(retryMessage, exception, span, arg3, arg4)))
                .ExecuteAsync(asyncFunc, new Context());
        }

        public static async Task HandleWaitAndRetryStorageOperationAsync(this IStorageService source,
            IConfigurationService configuration, ILogger logger, string retryMessage, Func<Context, Task> asyncFunc)
        {
            await Policy.Handle<StorageException>().WaitAndRetryAsync(
                    configuration["MaximumRetries"].SafeToInt(5),
                    (i, context) =>
                        TimeSpan.FromMilliseconds(Math.Pow(configuration["LeaseRetryInterval"].SafeToInt(50), i)),
                    (exception, span, arg3, arg4) =>
                        logger.LogDebug(string.Format(retryMessage, exception, span, arg3, arg4)))
                .ExecuteAsync(asyncFunc, new Context());
        }
    }
}