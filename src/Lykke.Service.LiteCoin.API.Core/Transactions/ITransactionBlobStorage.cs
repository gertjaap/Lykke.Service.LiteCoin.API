using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Transactions
{
    public enum TransactionBlobType
    {
        Initial = 0,
        Signed = 1
    }

    public interface ITransactionBlobStorage
    {
        Task<string> GetTransaction(string transactionId, TransactionBlobType type);

        Task AddOrReplaceTransaction(string txHash, TransactionBlobType type, string transactionHex);
    }
}
