using System;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{
    public interface ICashOutOperation
    {
        Guid OperationId { get; }
        DateTime StartedAt { get; }
        string ClientAddress { get;  }

        string AssetId { get; }

        long Amount { get; }

        string DestinationAddress { get; }

        string TxHash { get; }
    }

    public class CashOutOperation: ICashOutOperation
    {
        public Guid OperationId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string ClientAddress { get; set; }
        public string AssetId { get; set; }
        public long Amount { get; set; }
        public string DestinationAddress { get; set; }
        public string TxHash { get; set; }
        
        public static CashOutOperation Create(Guid operationId, 
            string walletId, 
            string address, 
            long amount,
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
                ClientAddress = walletId,
                TxHash = txHash
               
            };
        }
    }


    public interface ICashOutOperationRepository
    {
        Task<bool> Exist(Guid operationId);
        Task Insert(ICashOutOperation operation);
        Task<ICashOutOperation> GetByOperationId(Guid operationId);
        Task DeleteOldOperations(DateTime boun);
    }
}
