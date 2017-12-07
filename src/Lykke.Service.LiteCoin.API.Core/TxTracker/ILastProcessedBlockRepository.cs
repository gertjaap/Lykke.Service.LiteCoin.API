using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.TxTracker
{

    public interface ILastProcessedBlockRepository
    {
        Task<int> GetLastProcessedBlockHeight();
        Task SetLastProcessedBlockHeight(int height);
    }
}
