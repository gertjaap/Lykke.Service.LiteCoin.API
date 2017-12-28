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
        Task<string> GetTransaction(Guid operationId, TransactionBlobType type);

        Task AddOrReplaceTransaction(Guid operationId, TransactionBlobType type, string transactionHex);
    }
}
