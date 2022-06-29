using System.Threading.Tasks;

namespace Tools.Belt.Azure.ServiceBus.DNS.Management
{
    public interface IServiceBusQueueManager
    {
        Task<long> GetActiveMessageCountAsync(string queueName);
    }
}