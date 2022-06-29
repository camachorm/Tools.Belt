using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Azure.ServiceBus.DNS.Management
{

    public class ServiceBusQueueManager : IServiceBusQueueManager
    {
        private readonly ILogger<ServiceBusQueueManager> _logger;
        private readonly IServiceBusNamespace _serviceBusNamespace;

        public ServiceBusQueueManager(
            IServiceBusManagerConfiguration configuration,
            ILogger<ServiceBusQueueManager> logger)
        {
            _logger = logger;

            var managementTokenProvider = new CustomTokenProvider("https://management.core.windows.net/", configuration.TenantId);
            var managementTokenCredentials = new TokenCredentials(managementTokenProvider);

            AzureCredentials newCredentials = new AzureCredentials(managementTokenCredentials, managementTokenCredentials,
                configuration.TenantId, AzureEnvironment.AzureGlobalCloud);

            IServiceBusManager serviceBusManager =
                ServiceBusManager.Authenticate(newCredentials, configuration.SubscriptionId);

            _serviceBusNamespace = serviceBusManager.Namespaces
                .GetByResourceGroupAsync(configuration.ServiceBusResourceGroup, configuration.ServiceBusResourceName)
                .ReturnAsyncCallSynchronously();
        }

        public async Task<long> GetActiveMessageCountAsync(string queueName)
        {
            IQueue queue = await _serviceBusNamespace.Queues.GetByNameAsync(queueName);

            _logger.LogDebug(
                $"Queue {queue.Name} still contains a total of: {queue.ActiveMessageCount} Messages pending");

            return queue.ActiveMessageCount;
        }
    }
}

