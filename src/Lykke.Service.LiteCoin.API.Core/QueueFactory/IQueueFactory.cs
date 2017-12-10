using System;
using System.Collections.Generic;
using System.Text;
using AzureStorage.Queue;

namespace Lykke.Service.LiteCoin.API.Core.QueueFactory
{
    public interface IQueueFactory
    {
        IQueueExt GetQueue(string queueName);
    }
}
