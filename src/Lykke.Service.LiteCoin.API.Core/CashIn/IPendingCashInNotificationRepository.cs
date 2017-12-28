using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{
    public interface IPendingCashInNotification:ICashInOperation
    {
        
    }

    public class PendingCashInNotification : IPendingCashInNotification
    {
        public Guid OperationId { get; set; }

        public DateTime DetectedAt { get; set; }

        public string DestinationAddress { get; set; }

        public decimal Amount { get; set; }

        public string AssetId { get; set; }

        public string SourceAddress { get; set; }

        public string TxHash { get; set; }

        public static PendingCashInNotification Create(ICashInOperation source)
        {
            return new PendingCashInNotification
            {
                OperationId = source.OperationId,
                AssetId = source.AssetId,
                Amount = source.Amount,
                TxHash = source.TxHash,
                DetectedAt = source.DetectedAt,
                DestinationAddress = source.DestinationAddress,
                SourceAddress = source.SourceAddress
            };
        }
    }

    public interface IPendingCashInNotificationRepository
    {
        Task Insert(IPendingCashInNotification notification);
        Task<IEnumerable<IPendingCashInNotification>> GetAll();
        Task RemoveBatch(IEnumerable<Guid> operationIds);
    }
}
