using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.LiteCoin.API.Core;

namespace Lykke.Service.LiteCoin.API.Services.WebHook.Contracts
{
    internal class WebHookCashInRequestContract: IBaseWebHookRequestContract
    {
        public string OperationId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public CashInContextContract Context { get; set; }
    }

    class CashInContextContract
    {
        public string AssetId { get; set; }

        public decimal Amount { get; set; }
    }
}
