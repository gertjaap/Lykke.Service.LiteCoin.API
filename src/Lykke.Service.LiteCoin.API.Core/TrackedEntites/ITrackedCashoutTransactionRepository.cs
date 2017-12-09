using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.TrackedEntites
{
    public interface ITrackedCashoutTransaction
    {
        string TxHash { get; }
        string OperationId { get; }
        DateTime InsertedAt { get; }
    }

    public class TrackedCashOutTransaction: ITrackedCashoutTransaction
    {
        public string TxHash { get; set; }
        public string OperationId { get; set; }
        public DateTime InsertedAt { get; set; }


        public static TrackedCashOutTransaction Create(string txHash, string operationId, DateTime? insertedAt = null)
        {
            return new TrackedCashOutTransaction
            {
                OperationId = operationId,
                TxHash = txHash,
                InsertedAt = insertedAt ?? DateTime.UtcNow
            };
        }
    }
    public interface ITrackedCashoutTransactionRepository
    {
        Task<IEnumerable<ITrackedCashoutTransaction>> GetAll();
        Task Insert(ITrackedCashoutTransaction tx);
        Task Remove(string txHash);
    }
}
