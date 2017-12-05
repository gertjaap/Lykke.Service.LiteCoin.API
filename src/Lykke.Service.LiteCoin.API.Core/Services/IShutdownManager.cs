using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}