using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{
    public interface IDetectedAddressTransaction
    {
        string TransactionHash { get; }

        string Address { get; }

        DateTime InsertedAt { get; }
    }

    public class DetectedAddressTransaction : IDetectedAddressTransaction
    {
        public string TransactionHash { get; set; }
        public string Address { get; set; }
        public DateTime InsertedAt { get; set; }

        public static DetectedAddressTransaction Create(string txHash, string address, DateTime? insertedAt = null)
        {
            return new DetectedAddressTransaction
            {
                Address = address,
                TransactionHash = txHash,
                InsertedAt = insertedAt ?? DateTime.UtcNow
            };
        }
    }

    public interface IDetectedAddressTransactionsRepository
    {
        Task InsertIfNotExist(IEnumerable<IDetectedAddressTransaction> transactions);

        Task<IEnumerable<IDetectedAddressTransaction>> GetTxsForAddress(string address);
    }
}
