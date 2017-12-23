using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
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
        private readonly ILog _log;

        public SignService(IBlockchainSignServiceApiProvider serviceApiProvider,
            IWalletService walletService, 
            ILog log)
        {
            _serviceApiProvider = serviceApiProvider;
            _walletService = walletService;
            _log = log;
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
            try
            {
                return await _serviceApiProvider.SignTransaction(unsignedTransaction, publicAddress.Select(p => p.ToString()).ToArray());
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(SignService), nameof(SignTransaction),
                    unsignedTransaction.ToHex(), e);

                throw new BackendException("Sign error", ErrorCode.SignError);
            }
            
        }
    }
}
