using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.BlockChainReaders
{
    public interface IBlockChainReader
    {
        Task<IEnumerable<string>> GetTransactionsForAddress(string address, int minBlockHeight);
        Task<int> GetLastBlockHeight();
        Task<Transaction> GetRawTx(string tx);
    }
}
