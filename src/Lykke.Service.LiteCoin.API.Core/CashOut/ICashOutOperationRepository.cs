using System;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{
    public interface ICashOutOperation
    {
        string OperationId { get; }
        DateTime StartedAt { get; }
        string ClientWalletId { get;  }

        string AssetId { get; }

        decimal Amount { get; }

        string DestinationAddress { get; }

        string TxHash { get; }
    }

    public class CashOutOperation: ICashOutOperation
    {
        public string OperationId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string ClientWalletId { get; set; }
        public string AssetId { get; set; }
        public decimal Amount { get; set; }
        public string DestinationAddress { get; set; }
        public string TxHash { get; set; }
        
        public static CashOutOperation Create(string operationId, 
            string walletId, 
            string address, 
            decimal amount,
            string assetId,
            DateTime startedAt,
            string txHash )
        {
            return new CashOutOperation
            {
                DestinationAddress = address,
                Amount = amount,
                AssetId = assetId,
                OperationId = operationId,
                StartedAt = startedAt,
                ClientWalletId = walletId,
                TxHash = txHash
               
            };
        }
    }


    public interface ICashOutOperationRepository
    {

        Task Insert(ICashOutOperation operation);
        Task<ICashOutOperation> GetByOperationId(string operationId);
        Task DeleteOldOperations(DateTime boun);
    }
}
