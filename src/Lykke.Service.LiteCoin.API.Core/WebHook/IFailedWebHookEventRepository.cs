using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.WebHook
{
    public interface IFailedWebHookEventRepository
    {
        Task Insert(object  eventData, string operationId);
        Task DeleteIfExist(string operationId);
    }
}
