using System;

namespace Lykke.Service.LiteCoin.API.Services.WebHook.Contracts
{
    internal interface IBaseWebHookRequestContract
    {
        string OperationId { get; set; }

        DateTime DateTime { get; set; }

        string WalletId { get; set; }

        string Type { get; set; }
    }


}
