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
        public bool IsClientWallet { get; set; }

        public static Wallet Create(BitcoinAddress address, bool isClientWallet)
        {
            if (address != null)
            {
                return new Wallet
                {
                    Address = address,
                    IsClientWallet = isClientWallet
                };
            }

            return null;
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

            var hotWallets = _hotWalletsSettings.SourceWalletPublicAddresses.Distinct().ToDictionary(p => p);

            return allWallets.Where(p => !hotWallets.ContainsKey(p)).Select(p=>Wallet.Create(_addressValidator.GetBitcoinAddress(p), IsClientWallet(p))).ToList();
        }

        public async Task<IEnumerable<IWallet>> GetHotWallets()
        {
            var allWallets = await _serviceApiProvider.GetAllWallets();

            var hotWallets = _hotWalletsSettings.SourceWalletPublicAddresses.Distinct().ToDictionary(p => p);

            return allWallets.Where(p => hotWallets.ContainsKey(p)).Select(p => Wallet.Create(_addressValidator.GetBitcoinAddress(p), IsClientWallet(p))).ToList();
        }
        

        public async Task<IWallet> CreateWallet()
        {
            var resp = await _serviceApiProvider.CreateWallet();

            return Wallet.Create(_addressValidator.GetBitcoinAddress(resp), IsClientWallet(resp));
        }

        public async Task<IWallet> GetByPublicAddress(string address)
        {
            var resp = await _serviceApiProvider.GetByPublicAddress(address);
            return Wallet.Create(_addressValidator.GetBitcoinAddress(resp), IsClientWallet(resp));
        }

        private bool IsClientWallet(string address)
        {
            return _hotWalletsSettings.SourceWalletPublicAddresses.All(x => !string.Equals(x, address, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
