using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSignService.Client;
using Lykke.Service.BlockchainSignService.Client.Models;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Sign;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Sign
{
    public class BlockchainSignServiceApiProvider: IBlockchainSignServiceApiProvider
    {
        private readonly BlockchainSignServiceClient _client;

        public BlockchainSignServiceApiProvider(BlockchainSignServiceClient client)
        {
            _client = client;
        }

        public async Task<Transaction> SignTransaction(Transaction unsignedTransaction, params string[] publicAddresses)
        {
            foreach (var publicAddress in publicAddresses)
            {
                var wallet = await _client.GetWalletByPublicAddressAsync(publicAddress);

                if (wallet == null)
                {
                    throw new BusinessException("Cant find address to signTx", ErrorCode.CantFindAddressToSignTx);
                }
            }

            var signed = await _client.SignTransactionAsync(new SignRequestModel(publicAddresses, unsignedTransaction.ToHex()));

            return Transaction.Parse(signed.SignedTransaction);
        }

        public async Task<IEnumerable<string>> GetAllWallets()
        {
            return (await _client.GetAllWalletsAsync()).Select(p => p.PublicAddress);
        }

        public async Task<string> CreateWallet()
        {
            return (await _client.CreateWalletAsync()).PublicAddress;
        }

        public async Task<string> GetByPublicAddress(string address)
        {
            return (await _client.GetWalletByPublicAddressAsync(address))?.PublicAddress;
        }
    }
}
