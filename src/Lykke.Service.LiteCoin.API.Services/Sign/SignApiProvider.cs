using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Sign;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Sign
{
    public class SignApiProvider: ISignApiProvider
    {
        public Task<Transaction> SignTransaction(Transaction unsignedTransaction, params string[] walletIds)
        {
            throw new NotImplementedException();
        }

        public Task<(string walletId, string blockChainAddress)> GetByBlockChainAddress(string blockChainAddress)
        {
            throw new NotImplementedException();
        }

        public Task<(string walletId, string blockChainAddress)> GetByWalletId(string walletId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(string walletId, string blockChainAddress)>> GetAllWallets()
        {
            throw new NotImplementedException();
        }
    }
}
