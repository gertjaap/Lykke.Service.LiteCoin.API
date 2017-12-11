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

        public DateTime DateTime { get; set; }

        public string WalletId { get; set; }

        public string AssetId { get; set; }

        public decimal Amount { get; set; }

        public string DestAddress { get; set; }

        public string TxHash { get; set; }
    }
}
