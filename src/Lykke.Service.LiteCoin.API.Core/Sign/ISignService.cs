using System;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Sign
{
    public interface ISignService
    {
        Task<Transaction> SignTransaction(Guid operationId, Transaction unsignedTransaction, params BitcoinAddress[] publicAddress);
    }
}
