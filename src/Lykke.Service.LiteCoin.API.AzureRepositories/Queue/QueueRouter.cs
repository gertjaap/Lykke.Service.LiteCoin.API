using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;
using Lykke.Service.LiteCoin.API.Core.Queue;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Queue
{
    public class QueueRouter<T> : IQueueRouter<T> where T:IRoutedQueueMessageContext
    {
        private readonly IQueueFactory _queueFactory;

        public QueueRouter(IQueueFactory queueFactory)
        {
            _queueFactory = queueFactory;
        }

        public Task AddMessage(T message)
        {
            return _queueFactory.GetQueue(message.SendToQueue).PutRawMessageAsync(message.ToJson());
        }
    }
}
