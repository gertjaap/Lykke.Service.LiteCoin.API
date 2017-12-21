using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Wallet
{
    public interface IWallet
    {
        BitcoinAddress Address { get; }
        
    }

    public interface IWalletService
    {
        Task<IEnumerable<IWallet>> GetClientWallets();

        Task<IEnumerable<IWallet>> GetHotWallets();
        Task<IWallet> CreateWallet();
        Task<IWallet> GetByPublicAddress(string address);
    }
}
