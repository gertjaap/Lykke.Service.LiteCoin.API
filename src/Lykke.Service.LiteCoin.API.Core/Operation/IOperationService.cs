using System;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Operation
{
    public interface IOperationService
    {
        Task<Transaction> BuildTransferTransaction(Guid operationId, 
            BitcoinAddress fromAddress,
            BitcoinAddress toAddress,
            string assetId,
            Money amount, 
            bool includeFee);
    }
}
