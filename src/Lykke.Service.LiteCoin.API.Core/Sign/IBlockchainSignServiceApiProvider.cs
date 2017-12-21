using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Sign
{
    public interface IBlockchainSignServiceApiProvider
    {
        Task<Transaction> SignTransaction(Transaction unsignedTransaction, params string[] publicAddresses);
        
        Task<IEnumerable<string>> GetAllWallets();
        Task<string> CreateWallet();
        Task<string> GetByPublicAddress(string address);
    }
}
