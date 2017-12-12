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

        string WalletId { get; }

        double AmountSatoshi { get; }

        string AssetId { get; }

        string Address { get; }

        string TxHash { get; }
    }

    public class CashInOperation : ICashInOperation
    {
        public string OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string WalletId { get; set; }
        public double AmountSatoshi { get; set; }
        public string AssetId { get; set; }
        public string Address { get; set; }
        public string TxHash { get; set; }

        public static CashInOperation Create(string operationId, 
            string walletId,
            string address,
            string txHash,
            double amount, 
            string assetId, 
            DateTime detectedAt)
        {
            return new CashInOperation
            {
                OperationId = operationId,
                AssetId = assetId,
                Address = address,
                AmountSatoshi = amount,
                TxHash = txHash,
                WalletId = walletId,
                DetectedAt = detectedAt
            };
        }
    }

    public interface ICashInOperationRepository
    {
        Task Insert(ICashInOperation operation);
    }
}
