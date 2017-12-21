using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSignService.Client;
using Lykke.Service.BlockchainSignService.Client.Models;
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
            var guids = new List<Guid>();
            foreach (var publicAddress in publicAddresses)
            {
                var wallet = await _client.GetWalletByPublicAddressAsync(publicAddress);
                guids.Add(wallet.WalletId);
                
            }
            var signed = await _client.SignTransactionAsync(new SignRequestModel(guids, unsignedTransaction.ToHex()));

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
