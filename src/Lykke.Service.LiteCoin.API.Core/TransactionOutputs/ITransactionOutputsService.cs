using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs
{
    public interface ITransactionOutputsService
    {
        Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int confirmationsCount = 0);
        Task<IEnumerable<Coin>> GetOnlyBlockChainUnspentOutputs(string address, int confirmationsCount = 0);
    }
}
