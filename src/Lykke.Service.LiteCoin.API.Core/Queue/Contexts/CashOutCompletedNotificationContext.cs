using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Queue.Contexts
{
    public class CashOutCompletedNotificationContext : IRoutedQueueMessageContext
    {
        public const string QueueName = "cash-out-completed-notifications";
        public string SendToQueue => QueueName;

        public string OperationId { get; set; }
    }
}
