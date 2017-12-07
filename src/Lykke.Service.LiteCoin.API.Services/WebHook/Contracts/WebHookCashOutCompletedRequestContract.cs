using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.LiteCoin.API.Core;

namespace Lykke.Service.LiteCoin.API.Services.WebHook.Contracts
{
    internal class WebHookCashOutCompletedRequestContract : IBaseWebHookRequestContract
    {
        public string OperationId { get; set; }
        public DateTime DateTime { get; set; }
        public string WalletId { get; set; }
        public string Type { get; set; }

        public CashOutCompletedContextContract Context { get; set; }
    }

    class CashOutCompletedContextContract
    {
        public string AssetId { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }
    }
}
