using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Operation;

namespace Lykke.Service.LiteCoin.API.Core.PendingEvent
{
    public enum BroadcastStatus
    {
        InProgress,
        Completed,
        Failed
    }

    public interface IObservableTransactionData 
    {
        BroadcastStatus Status { get; }
        Guid OperationId { get;  }
        string FromAddress { get; }
        string ToAddress { get;  }
        string AssetId { get;  }
        long AmountSatoshi { get;  }
        DateTime Updated { get;  }
    }

    public class ObervableTransactionData : IObservableTransactionData
    {
        public Guid OperationId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public long AmountSatoshi { get; set; }
        public bool IncludeFee { get; set; }
        public DateTime Updated { get; set; }
        public BroadcastStatus Status { get; set; }

        public static ObervableTransactionData Create(IOperationMeta operation, BroadcastStatus status, DateTime? updated = null)
        {
            return new ObervableTransactionData
            {
                OperationId = operation.OperationId,
                AmountSatoshi = operation.AmountSatoshi,
                AssetId = operation.AssetId,
                FromAddress = operation.FromAddress,
                IncludeFee = operation.IncludeFee,
                ToAddress = operation.ToAddress,
                Status = status,
                Updated = updated ?? DateTime.UtcNow
            };
        }
    }
 
    public interface IObservableOperationRepository
    {
        Task<IEnumerable<IObservableTransactionData>> Get(BroadcastStatus status, int skip, int take);
        Task Insert(IObservableTransactionData tx);
        Task ChangeStatus(Guid operationId, BroadcastStatus status);
        Task DeleteIfExist(params Guid[] operationIds);
    }
}
