using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Queue.Contexts
{
    public class SendCashInToHotWalletContext: IRoutedQueueMessageContext
    {
        public const string QueueName = "send-cash-in-to-hot-wallet";
        public string SendToQueue => QueueName;
        
        public Guid OperationId { get; set; }
    }
}
