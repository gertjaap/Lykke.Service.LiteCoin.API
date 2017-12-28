using System;

namespace Lykke.Service.LiteCoin.API.Services.WebHook.Contracts
{
    internal interface IBaseWebHookRequestContract
    {
        string OperationId { get; set; }

        DateTime TimeStamp { get; set; }

        string Address { get; set; }

        string Type { get; set; }
    }


}
