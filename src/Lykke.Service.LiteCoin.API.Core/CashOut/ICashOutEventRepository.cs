using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.CashOut
{

    public enum CashOutEventType
    {
       NotificationOnStartSend,
       DetectedOnBlockChain,
       NotificationOnCompletedSend
    }

    public interface ICashOutEvent
    {
        CashOutEventType Type { get; }

        DateTime DateTime { get; }

        string OperationId { get; }
    }

    public class CashOutEvent : ICashOutEvent
    {
        public CashOutEventType Type { get; set; }
        public DateTime DateTime { get; set; }
        public string OperationId { get; set; }

        public static CashOutEvent Create(string operationId, CashOutEventType type, DateTime? dateTime = null)
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
        Task<bool> Exist(string operationId, CashOutEventType type);
    }
}
