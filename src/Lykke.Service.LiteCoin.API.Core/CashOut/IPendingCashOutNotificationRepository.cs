using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{
    public enum CashOutStatusType
    {
        Started,
        Completed,
        Failed
    }

    public interface IPendingCashOutNotification : ICashOutOperation
    {
        CashOutStatusType Status { get; }
    }

    public class PendingCashOutNotification: IPendingCashOutNotification
    {
        public Guid OperationId { get; set; }
        public DateTime StartedAt { get; set; }
        public string ClientWalletId { get; set; }
        public string AssetId { get; set; }
        public decimal Amount { get; set; }
        public string DestinationAddress { get; set; }
        public string TxHash { get; set; }
        public CashOutStatusType Status { get; set; }

        public static PendingCashOutNotification Create(ICashOutOperation source, CashOutStatusType type)
        {
            return new PendingCashOutNotification
            {
                OperationId = source.OperationId,
                Amount = source.Amount,
                AssetId = source.AssetId,
                TxHash = source.TxHash,
                StartedAt = source.StartedAt,
                Status = type,
                ClientWalletId = source.ClientWalletId,
                DestinationAddress = source.DestinationAddress
            };
        }
    }

    public interface IPendingCashOutNotificationRepository
    {
        Task InsertOrReplace(IPendingCashOutNotification notification);
        Task<IEnumerable<IPendingCashOutNotification>> GetAll();
        Task RemoveBatch(IEnumerable<Guid> operationIds);
    }
}
