using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Queue.Contexts
{
    public class CashInNotificationContext:IRoutedQueueMessageContext
    {
        public const string QueueName = "cash-in-notifications";
        public string SendToQueue => QueueName;

        public string OperationId { get; set; }
    }
}
