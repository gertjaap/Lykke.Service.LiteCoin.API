using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.BlockChainReaders
{
    public interface IBlockChainProvider
    {
        Task<IEnumerable<string>> GetTransactionsForAddress(string address);
        Task<IEnumerable<string>> GetTransactionsForAddress(BitcoinAddress address);
        Task<int> GetLastBlockHeight();
        Task<Transaction> GetRawTx(string tx);
        Task BroadCastTransaction(Transaction tx);
        Task<int> GetTxConfirmationCount(string txHash);
        Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int minConfirmationCount);
        Task<string> GetDestinationAddress(string txHash, uint n);
        Task<long> GetBalanceSatoshi(string address);
    }
}
