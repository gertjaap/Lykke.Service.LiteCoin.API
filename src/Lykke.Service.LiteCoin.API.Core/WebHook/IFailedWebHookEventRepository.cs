using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.WebHook
{
    public interface IFailedWebHookEventRepository
    {
        Task InsertAsync(object  eventData, string operationId);
    }
}
