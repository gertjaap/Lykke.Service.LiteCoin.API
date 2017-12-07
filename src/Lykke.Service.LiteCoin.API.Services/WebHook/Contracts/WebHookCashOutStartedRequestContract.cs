using System;

namespace Lykke.Service.LiteCoin.API.Services.WebHook.Contracts
{
    internal class WebHookCashOutStartedRequestContract : IBaseWebHookRequestContract
    {
        public string OperationId { get; set; }
        public DateTime DateTime { get; set; }
        public string WalletId { get; set; }
        public string Type { get; set; }

        public CashOutStartedContextContract Context { get; set; }
    }

    class CashOutStartedContextContract
    {
        public string AssetId { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }
    }
}
