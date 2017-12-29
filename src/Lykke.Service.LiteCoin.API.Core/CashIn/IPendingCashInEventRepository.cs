using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{
    public enum PendingCashInEventStatusType
    {
        DetectedOnBlockChain
    }

    public interface IPendingCashInEvent:ICashInOperation
    {
        PendingCashInEventStatusType Status { get; }
    }

    public class PendingCashInEvent : IPendingCashInEvent
    {
        public Guid OperationId { get; set; }

        public DateTime DetectedAt { get; set; }

        public string DestinationAddress { get; set; }

        public long Amount { get; set; }

        public string AssetId { get; set; }

        public string SourceAddress { get; set; }

        public string TxHash { get; set; }
        public PendingCashInEventStatusType Status { get; set; }

        public static PendingCashInEvent Create(ICashInOperation source, PendingCashInEventStatusType status)
        {
            return new PendingCashInEvent
            {
                OperationId = source.OperationId,
                AssetId = source.AssetId,
                Amount = source.Amount,
                TxHash = source.TxHash,
                DetectedAt = source.DetectedAt,
                DestinationAddress = source.DestinationAddress,
                SourceAddress = source.SourceAddress,
                Status = status
            };
        }

    }

    public interface IPendingCashInEventRepository
    {
        Task Insert(IPendingCashInEvent @event);
        Task<IEnumerable<IPendingCashInEvent>> GetAll(PendingCashInEventStatusType status, int count);
        Task DeleteBatchIfExist(PendingCashInEventStatusType status, IEnumerable<Guid> operationIds);
    }
}
