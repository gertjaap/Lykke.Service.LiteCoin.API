using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.Sign;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Services.SourceWallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Wallet
{
    public class Wallet : IWallet
    {
        public BitcoinAddress Address { get; set; }
        public string WalletId { get; set; }

        public static Wallet Create(BitcoinAddress address, string walletId)
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
        private readonly IBlockchainSignServiceApiProvider _serviceApiProvider;
        private readonly IAddressValidator _addressValidator;

        public WalletService(HotWalletsSettings hotWalletsSettings, IBlockchainSignServiceApiProvider serviceApiProvider, IAddressValidator addressValidator)
        {
            _hotWalletsSettings = hotWalletsSettings;
            _serviceApiProvider = serviceApiProvider;
            _addressValidator = addressValidator;
        }

        public async Task<IEnumerable<IWallet>> GetClientWallets()
        {
            var allWallets = await _serviceApiProvider.GetAllWallets();

            var hotWallets = _hotWalletsSettings.SourceWalletIds.Distinct().ToDictionary(p => p);

            return allWallets.Where(p => !hotWallets.ContainsKey(p.walletId)).Select(p=>Wallet.Create(_addressValidator.GetBitcoinAddress(p.blockChainAddress), p.walletId)).ToList();
        }

        public async Task<IEnumerable<IWallet>> GetHotWallets()
        {
            var allWallets = await _serviceApiProvider.GetAllWallets();

            var hotWallets = _hotWalletsSettings.SourceWalletIds.Distinct().ToDictionary(p => p);

            return allWallets.Where(p => hotWallets.ContainsKey(p.walletId)).Select(p => Wallet.Create(_addressValidator.GetBitcoinAddress(p.blockChainAddress), p.walletId)).ToList();
        }

        public async Task<IWallet> GetByWalletId(string walletId)
        {
            var resp = await _serviceApiProvider.GetByWalletId(walletId);

            return Wallet.Create(_addressValidator.GetBitcoinAddress(resp.blockChainAddress), resp.walletId);
        }

        public async Task<IWallet> CreateWallet()
        {
            var resp = await _serviceApiProvider.CreateWallet();

            return Wallet.Create(_addressValidator.GetBitcoinAddress(resp.blockChainAddress), resp.walletId);
        }
    }
}
