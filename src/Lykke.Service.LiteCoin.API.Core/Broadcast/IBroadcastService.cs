using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Broadcast
{
    public interface IBroadcastService
    {
        Task BroadCastTransaction(Transaction tx);
    }
}
