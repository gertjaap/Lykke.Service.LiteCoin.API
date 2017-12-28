using System;

namespace Lykke.Service.LiteCoin.API.Services.WebHook.Contracts
{
    internal class WebHookCashOutStartedRequestContract : IBaseWebHookRequestContract
    {
        public string OperationId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Address { get; set; }
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
