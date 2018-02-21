using System;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.Vertcoin.API.Core.Broadcast
{
    public interface IBroadcastService
    {
        Task BroadCastTransaction(Guid operationId, string txHex);
        Task BroadCastTransaction(Guid operationId, Transaction transaction);
    }
}
