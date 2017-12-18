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

        decimal Amount { get; }

        string AssetId { get; }

        string Address { get; }

        string TxHash { get; }

        bool MoneyTransferredToHotWallet { get; }

        DateTime? MoneyTransferredToHotWalletAt { get; }
    }

    public class CashInOperation : ICashInOperation
    {
        public string OperationId { get; set; }
        public DateTime DetectedAt { get; set; }
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string Address { get; set; }
        public string TxHash { get; set; }
        public bool MoneyTransferredToHotWallet { get; set; }
        public DateTime? MoneyTransferredToHotWalletAt { get; set; }

        public static CashInOperation Create(string operationId, 
            string walletId,
            string address,
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
                Address = address,
                Amount = amount,
                TxHash = txHash,
                WalletId = walletId,
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
