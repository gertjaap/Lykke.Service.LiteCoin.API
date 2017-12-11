using AzureStorage.Queue;

namespace Lykke.Service.LiteCoin.API.Core.Queue
{
    public interface IQueueFactory
    {
        IQueueExt GetQueue(string queueName);
    }
}
