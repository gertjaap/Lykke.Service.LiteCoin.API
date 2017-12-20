using System.Collections.Generic;
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

        public async Task<Transaction> SignTransaction(Transaction unsignedTransaction, params string[] walletIds)
        {
            return await _serviceApiProvider.SignTransaction(unsignedTransaction, walletIds);
        }
    }
}
