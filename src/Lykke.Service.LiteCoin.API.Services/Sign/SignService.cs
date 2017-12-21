using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Sign;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Sign
{
    public class SignService: ISignService
    {
        private readonly IBlockchainSignServiceApiProvider _serviceApiProvider;
        private readonly IWalletService _walletService;

        public SignService(IBlockchainSignServiceApiProvider serviceApiProvider, IWalletService walletService)
        {
            _serviceApiProvider = serviceApiProvider;
            _walletService = walletService;
        }

        public async Task<Transaction> SignTransaction(Transaction unsignedTransaction, params BitcoinAddress[] publicAddress)
        {
            foreach (var bitcoinAddress in publicAddress)
            {
                var wallet = await _walletService.GetByPublicAddress(bitcoinAddress.ToString());
                if (wallet == null)
                {
                    throw new BackendException($"Wallet {bitcoinAddress} not found", ErrorCode.WalletNotFound);
                }
            }
            return await _serviceApiProvider.SignTransaction(unsignedTransaction, publicAddress.Select(p=>p.ToString()).ToArray());
        }
    }
}
