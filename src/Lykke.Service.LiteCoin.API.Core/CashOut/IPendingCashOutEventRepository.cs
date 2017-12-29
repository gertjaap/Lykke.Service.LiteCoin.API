using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.CashIn;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{
    public enum PendingCashOutEventStatusType
    {
        Started,
        Completed,
        Failed
    }

    public interface IPendingCashOutEvent:ICashOutOperation
    {
        PendingCashOutEventStatusType Status { get; }
    }

    public class PendingCashOutEvent : IPendingCashOutEvent
    {
        public Guid OperationId { get; set; }
        public DateTime StartedAt { get; set; }
        public string ClientWalletId { get; set; }
        public string AssetId { get; set; }
        public long Amount { get; set; }
        public string DestinationAddress { get; set; }
        public string TxHash { get; set; }
        public PendingCashOutEventStatusType Status { get; set; }

        public static PendingCashOutEvent Create(ICashOutOperation source, PendingCashOutEventStatusType status)
        {
            return new PendingCashOutEvent
            {
                OperationId = source.OperationId,
                Amount = source.Amount,
                AssetId = source.AssetId,
                TxHash = source.TxHash,
                StartedAt = source.StartedAt,
                ClientWalletId = source.ClientWalletId,
                DestinationAddress = source.DestinationAddress,
                Status = status
            };
        }

    }

    public interface IPendingCashOutEventRepository
    {
        Task Insert(IPendingCashOutEvent @event);
        Task<IEnumerable<IPendingCashOutEvent>> GetAll(PendingCashOutEventStatusType status, int count);
        Task DeleteBatchIfExist(PendingCashOutEventStatusType status, IEnumerable<Guid> operationIds);
    }
}
