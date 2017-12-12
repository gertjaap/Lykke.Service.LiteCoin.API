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
        private readonly ISignApiProvider _apiProvider;
        private readonly IWalletService _walletService;

        public SignService(ISignApiProvider apiProvider, IWalletService walletService)
        {
            _apiProvider = apiProvider;
            _walletService = walletService;
        }

        public async Task<Transaction> SignTransaction(Transaction unsignedTransaction, params BitcoinAddress[] signUsingAddresses)
        {
            var walletIds = new List<string>();
            foreach (var address in signUsingAddresses)
            {
                var wallet = await _walletService.GetByBlockChainAddress(address.ToString());
                if (wallet == null)
                {
                    throw new BackendException($"Cant find wallet {wallet.Address}", ErrorCode.CantFindAddressToSignTx);
                }

                walletIds.Add(wallet.WalletId);
            }

            return await _apiProvider.SignTransaction(unsignedTransaction, walletIds.ToArray());
        }
    }
}
