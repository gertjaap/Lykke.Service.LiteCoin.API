using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Wallet
{
    public interface IWallet
    {
        string Address { get; }

        string WalletId { get; }
    }

    public interface IWalletService
    {
        Task<IEnumerable<IWallet>> GetClientWallets();

        Task<IEnumerable<IWallet>> GetHotWallets();

        Task<IWallet> GetByWalletId(string walletId);

        Task<IWallet> GetByBlockChainAddress(string address);
    }
}
