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

        string DestinationWalletId { get; }

        decimal Amount { get; }

        string AssetId { get; }

        string SourceAddress { get; }

        string TxHash { get; }

        bool MoneyTransferredToHotWallet { get; }

        DateTime? MoneyTransferredToHotWalletAt { get; }
    }

    public class CashInOperation : ICashInOperation
    {
        public string OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string DestinationWalletId { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string SourceAddress { get; set; }
        public string TxHash { get; set; }
        public bool MoneyTransferredToHotWallet { get; set; }
        public DateTime? MoneyTransferredToHotWalletAt { get; set; }

        public static CashInOperation Create(string operationId, 
            string walletId,
            string sourceAddress,
            string txHash,
            decimal amount, 
            string assetId, 
            DateTime detectedAt,
            bool moneyTransferredToHotWallet = false,
            DateTime? moneyTransferredToHotWalletAt = null)
        {
            return new CashInOperation
            {
                OperationId = operationId,
                AssetId = assetId,
                SourceAddress = sourceAddress,
                Amount = amount,
                TxHash = txHash,
                DestinationWalletId = walletId,
                DetectedAt = detectedAt,
                MoneyTransferredToHotWallet = moneyTransferredToHotWallet,
                MoneyTransferredToHotWalletAt = moneyTransferredToHotWalletAt
            };
        }
    }

    public interface ICashInOperationRepository
    {
        Task Insert(ICashInOperation operation);
        Task<ICashInOperation> GetByOperationId(string operationId);
        Task DeleteOldOperations(DateTime bound);
        Task SetMoneyTransferred(string operationId, DateTime completedAt);
    }
}
