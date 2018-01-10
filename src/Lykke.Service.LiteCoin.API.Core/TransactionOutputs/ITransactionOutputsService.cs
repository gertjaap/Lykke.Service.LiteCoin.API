using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs
{
    public interface ITransactionOutputsService
    {
        Task<IEnumerable<CoinWithSettlementInfo>> GetUnspentOutputs(string address, int confirmationsCount = 0);
    }
    

}
