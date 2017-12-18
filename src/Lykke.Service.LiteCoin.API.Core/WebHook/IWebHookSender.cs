using System;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.WebHook
{
    public interface IWebHookSender
    {
        Task ProcessCashIn(string operationId, 
            DateTime dateTime,
            string walletId, 
            string assetId, 
            decimal amount, 
            string sourceAddress);

        Task ProcessCashOutStarted(string operationId,
            DateTime dateTime,
            string walletId,
            string assetId,
            decimal amount,
            string destAddress,
            string txHash);

        Task ProcessCashOutCompleted(string operationId,
            DateTime dateTime,
            string walletId,
            string assetId,
            decimal amount,
            string destAddress,
            string txHash);
    }
}
