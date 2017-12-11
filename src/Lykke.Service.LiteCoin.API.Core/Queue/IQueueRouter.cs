using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Queue
{
    public interface IRoutedQueueMessageContext
    {
        string SendToQueue { get; }
    }

    public interface IQueueRouter<T> where T:IRoutedQueueMessageContext
    {
        Task AddMessage(T message);
    }
}
