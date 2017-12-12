using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.BlockChainTracker
{

    public interface ICashInLastProcessedBlockRepository
    {
        Task<int> GetLastProcessedBlockHeight();
        Task SetLastProcessedBlockHeight(int height);
    }
}
