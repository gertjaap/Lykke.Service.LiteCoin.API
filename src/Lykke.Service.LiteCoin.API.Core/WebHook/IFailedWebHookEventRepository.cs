using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace Lykke.Service.LiteCoin.API.Core.WebHook
{
    public enum WebHookType
    {
        CashIn,
        CashOutStarted,
        CashOutCompleted
    }

    public interface IFailedWebHookEvent
    {
        string OperationId { get; }
        string Context { get; }
        WebHookType WebHookType { get; }
    }

    public class FailedWebHookEvent : IFailedWebHookEvent
    {
        public string OperationId { get; set; }
        public string Context { get; set; }
        public WebHookType WebHookType { get; set; }

        public static FailedWebHookEvent Create(string opId, object data, WebHookType type)
        {
            return new FailedWebHookEvent
            {
                Context = data.ToJson(),
                OperationId = opId,
                WebHookType = type
            };
        }
    }

    public interface IFailedWebHookEventRepository
    {
        Task Insert(IFailedWebHookEvent ev);
        Task DeleteIfExist(string operationId);
        Task<IEnumerable<IFailedWebHookEvent>> GetAll();
    }
}
