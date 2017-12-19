using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Sign
{
    public interface ISignApiProvider
    {
        Task<Transaction> SignTransaction(Transaction unsignedTransaction, params string[] walletIds);

        Task<(string walletId, string blockChainAddress)> GetByBlockChainAddress(string blockChainAddress);

        Task<(string walletId, string blockChainAddress)> GetByWalletId(string walletId);

        Task<IEnumerable<(string walletId, string blockChainAddress)>> GetAllWallets();


        Task<(string walletId, string blockChainAddress)> CreateWallet();
    }
}
