using System.Threading.Tasks;

namespace Lykke.Service.Vertcoin.API.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}