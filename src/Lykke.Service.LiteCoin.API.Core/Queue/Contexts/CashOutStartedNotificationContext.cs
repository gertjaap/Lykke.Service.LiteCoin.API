using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Queue.Contexts
{
    public class CashOutStartedNotificationContext : IRoutedQueueMessageContext
    {
        public const string QueueName = "cash-out-started-notifications";
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
