using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{
    public interface ICashInOperation
    {
        string OperationId { get; }

        DateTime DetectedAt { get; }

        string DestinationAddress { get; }

        decimal Amount { get; }

        string AssetId { get; }

        string SourceAddress { get; }

        string TxHash { get; }
    }

    public class CashInOperation : ICashInOperation
    {
        public string OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string DestinationAddress { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string SourceAddress { get; set; }
        public string TxHash { get; set; }

        public static CashInOperation Create(string operationId, 
            string destinationAddress,
            string sourceAddress,
            string txHash,
            decimal amount, 
            string assetId, 
            DateTime detectedAt)
        {
            return new CashInOperation
            {
                OperationId = operationId,
                AssetId = assetId,
                SourceAddress = sourceAddress,
                Amount = amount,
                TxHash = txHash,
                DestinationAddress = destinationAddress,
                DetectedAt = detectedAt
            };
        }
    }

    public interface ICashInOperationRepository
    {
        Task Insert(ICashInOperation operation);
        Task<ICashInOperation> GetByOperationId(string operationId);
        Task<ICashInOperation> GetByTxHash(string txHash);
        Task DeleteOldOperations(DateTime bound);
    }
}
