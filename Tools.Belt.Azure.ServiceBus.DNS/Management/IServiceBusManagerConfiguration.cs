using Tools.Belt.Azure.DNS.Abstractions.Configuration.AzureActiveDirectory;

namespace Tools.Belt.Azure.ServiceBus.DNS.Management
{
    public interface IServiceBusManagerConfiguration : IAzureActiveDirectoryClientCredentialsConfiguration
    {
        /// <summary>
        ///     The name of the Azure Resource Group that contains our Service Bus.
        /// </summary>
        string ServiceBusResourceGroup { get; }

        /// <summary>
        ///     The name of our Azure Service Bus.
        /// </summary>
        string ServiceBusResourceName { get; }
    }
}