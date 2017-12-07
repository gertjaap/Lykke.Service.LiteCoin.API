using System;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.WebHook
{


    public interface IWebHookEvent
    {
        bool IsSuccess { get; }
        string Error { get; }
        
        object RequestData { get; }
    }
    
    public interface IWebHookSender
    {
        Task<IWebHookEvent> ProcessCashIn(string operationId, 
            DateTime dateTime,
            string walletId, 
            string assetId, 
            decimal amount, 
            string sourceAddress);

        Task<IWebHookEvent> ProcessCashOutStarted(string operationId,
            DateTime dateTime,
            string walletId,
            string assetId,
            decimal amount,
            string destAddress,
            string txHash);

        Task<IWebHookEvent> ProcessCashOutCompleted(string operationId,
            DateTime dateTime,
            string walletId,
            string assetId,
            decimal amount,
            string destAddress,
            string txHash);
    }
}
