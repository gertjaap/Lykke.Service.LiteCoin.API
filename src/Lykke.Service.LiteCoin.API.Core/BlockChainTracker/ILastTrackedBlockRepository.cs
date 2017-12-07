using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.BlockChainTracker
{

    public interface ILastTrackedBlockRepository
    {
        Task<int> GetLastProcessedBlockHeight();
        Task SetLastProcessedBlockHeight(int height);
    }
}
