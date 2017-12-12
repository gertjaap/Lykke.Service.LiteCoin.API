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

        public DateTime DateTime { get; set; }

        public string WalletId { get; set; }

        public string AssetId { get; set; }

        public double AmountSatoshi { get; set; }

        public string SourceAddress { get; set; }
    }
}
