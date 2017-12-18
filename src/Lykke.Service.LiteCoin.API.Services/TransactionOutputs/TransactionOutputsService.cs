using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.Helpers;
using NBitcoin;
using NBitcoin.OpenAsset;

namespace Lykke.Service.LiteCoin.API.Services.TransactionOutputs
{
    public class TransactionOutputsService : ITransactionOutputsService
    {
        private readonly Network _network;
        private readonly IBroadcastedOutputRepository _broadcastedOutputRepository;
        private readonly ISpentOutputRepository _spentOutputRepository;
        private readonly IBlockChainProvider _blockChainProvider;

        public TransactionOutputsService(Network network, IBroadcastedOutputRepository broadcastedOutputRepository, ISpentOutputRepository spentOutputRepository, IBlockChainProvider blockChainProvider)
        {
            _network = network;
            _broadcastedOutputRepository = broadcastedOutputRepository;
            _spentOutputRepository = spentOutputRepository;
            _blockChainProvider = blockChainProvider;
        }

        public async Task<IEnumerable<ICoin>> GetUnspentOutputs(string address, int confirmationsCount = 0)
        {
            var coins = (await _blockChainProvider.GetUnspentOutputs(address, confirmationsCount)).ToList();

            if (confirmationsCount == 0)
            {
                await AddBroadcastedOutputs(coins, address);
            }


            coins = await FilterCoins(coins);

            return coins;
        }

        public async Task<IEnumerable<ICoin>> GetOnlyBlockChainUnspentOutputs(string address, int confirmationsCount = 0)
        {
            var coins = (await _blockChainProvider.GetUnspentOutputs(address, confirmationsCount)).ToList();

            return coins;
        }

        private async Task AddBroadcastedOutputs(List<ICoin> coins, string walletAddress)
        {
            var set = new HashSet<OutPoint>(coins.Select(x => x.Outpoint));

            var internalSavedOutputs = (await _broadcastedOutputRepository.GetOutputs(walletAddress))
                .Where(o => !set.Contains(new OutPoint(uint256.Parse(o.TransactionHash), o.N)));

            coins.AddRange(internalSavedOutputs.Select(o =>
            {
                var coin = new Coin(new OutPoint(uint256.Parse(o.TransactionHash), o.N),
                    new TxOut(new Money(o.Amount, MoneyUnit.Satoshi), o.ScriptPubKey.ToScript()));

                return coin;
            }));
        }

        private async Task<List<ICoin>> FilterCoins(List<ICoin> coins)
        {
            var unspentOutputs = await _spentOutputRepository.GetUnspentOutputs(coins.Select(o => new Output(o.Outpoint)));

            var unspentSet = new HashSet<OutPoint>(unspentOutputs.Select(x => new OutPoint(uint256.Parse(x.TransactionHash), x.N)));


            
            return coins.Where(o => unspentSet.Contains(o.Outpoint)).ToList();
        }

    }
}
