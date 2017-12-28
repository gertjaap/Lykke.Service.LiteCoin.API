using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{

    public enum CashOutEventType
    {
       DetectedOnBlockChain
    }

    public interface ICashOutEvent
    {
        CashOutEventType Type { get; }

        DateTime DateTime { get; }

        Guid OperationId { get; }
    }

    public class CashOutEvent : ICashOutEvent
    {
        public CashOutEventType Type { get; set; }
        public DateTime DateTime { get; set; }
        public Guid OperationId { get; set; }

        public static CashOutEvent Create(Guid operationId, CashOutEventType type, DateTime? dateTime = null)
        {
            return new CashOutEvent
            {
                DateTime = dateTime ?? DateTime.UtcNow,
                OperationId = operationId,
                Type = type
            };
        }
    }

    public interface ICashOutEventRepository
    {
        Task InsertEvent(ICashOutEvent cashOutEvent);
        Task<bool> Exist(Guid operationId, CashOutEventType type);
    }
}
