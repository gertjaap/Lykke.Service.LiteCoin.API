using AzureStorage.Queue;
using Lykke.Service.LiteCoin.API.Core.QueueFactory;
using Lykke.SettingsReader;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Queue
{
    public class AzureQueueFactory:IQueueFactory
    {
        private readonly IReloadingManager<string> _connectionStringManager;

        public AzureQueueFactory(IReloadingManager<string> connectionStringManager)
        {
            _connectionStringManager = connectionStringManager;
        }

        public IQueueExt GetQueue(string queueName)
        {
            return AzureQueueExt.Create(_connectionStringManager, queueName);
        }
    }
}
