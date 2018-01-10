using System;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Operation
{

    public enum OperationEventType
    {
        Broadcasted,
        DetectedOnBlockChain
    }

    public interface IOperationEvent
    {
        OperationEventType Type { get; }

        DateTime DateTime { get; }

        Guid OperationId { get; }
    }

    public class OperationEvent : IOperationEvent
    {
        public OperationEventType Type { get; set; }
        public DateTime DateTime { get; set; }
        public Guid OperationId { get; set; }

        public static OperationEvent Create(Guid operationId, OperationEventType type, DateTime? dateTime = null)
        {
            return new OperationEvent
            {
                DateTime = dateTime ?? DateTime.UtcNow,
                OperationId = operationId,
                Type = type
            };
        }
    }

    public interface IOperationEventRepository
    {
        Task InsertIfNotExist(IOperationEvent operationEvent);
        Task<bool> Exist(Guid operationId, OperationEventType type);
    }
}
