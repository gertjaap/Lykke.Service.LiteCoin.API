using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs
{
    public interface IBroadcastedOutputsService
    {
        Task SaveNewOutputs(Transaction tr);
        Task<IEnumerable<IBroadcastedOutput>> GetOutputs(string address);
    }
}
