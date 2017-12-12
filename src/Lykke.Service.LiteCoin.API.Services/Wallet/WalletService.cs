using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Sign;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Services.SourceWallet;

namespace Lykke.Service.LiteCoin.API.Services.Wallet
{
    public class Wallet : IWallet
    {
        public string Address { get; set; }
        public string WalletId { get; set; }

        public static Wallet Create(string address, string walletId)
        {
            return new Wallet
            {
                Address = address,
                WalletId = walletId
            };
        }
    }
    public class WalletService: IWalletService
    {
        private readonly HotWalletsSettings _hotWalletsSettings;
        private readonly ISignApiProvider _apiProvider;

        public WalletService(HotWalletsSettings hotWalletsSettings, ISignApiProvider apiProvider)
        {
            _hotWalletsSettings = hotWalletsSettings;
            _apiProvider = apiProvider;
        }

        public async Task<IEnumerable<IWallet>> GetClientWallets()
        {
            var allWallets = await _apiProvider.GetAllWallets();

            var hotWallets = _hotWalletsSettings.SourceWalletIds.Distinct().ToDictionary(p => p);

            return allWallets.Where(p => !hotWallets.ContainsKey(p.walletId)).Select(p=>Wallet.Create(p.blockChainAddress, p.walletId)).ToList();
        }

        public async Task<IEnumerable<IWallet>> GetHotWallets()
        {
            var allWallets = await _apiProvider.GetAllWallets();

            var hotWallets = _hotWalletsSettings.SourceWalletIds.Distinct().ToDictionary(p => p);

            return allWallets.Where(p => hotWallets.ContainsKey(p.walletId)).Select(p => Wallet.Create(p.blockChainAddress, p.walletId)).ToList();
        }

        public async Task<IWallet> GetByWalletId(string walletId)
        {
            var resp = await _apiProvider.GetByWalletId(walletId);

            return Wallet.Create(resp.blockChainAddress, resp.walletId);
        }

        public async Task<IWallet> GetByBlockChainAddress(string address)
        {
            var resp = await _apiProvider.GetByBlockChainAddress(address);

            return Wallet.Create(resp.blockChainAddress, resp.walletId);
        }
    }
}
