using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.Helpers;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.TransactionOutputs
{
    public class TransactionOutputsService : ITransactionOutputsService
    {
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly ISpentOutputService _spentOutputService;
        private readonly IBroadcastedOutputsService _broadcastedOutputsService;

        public TransactionOutputsService(IBlockChainProvider blockChainProvider, 
            ISpentOutputService spentOutputService, 
            IBroadcastedOutputsService broadcastedOutputsService)
        {
            _blockChainProvider = blockChainProvider;
            _spentOutputService = spentOutputService;
            _broadcastedOutputsService = broadcastedOutputsService;
        }

        public async Task<IEnumerable<CoinWithSettlementInfo>> GetUnspentOutputs(string address, int confirmationsCount = 0)
        {
            var coins = (await _blockChainProvider.GetUnspentOutputs(address, confirmationsCount)).Select(p => CoinWithSettlementInfo.Create(p, isSettled: true)).ToList();
            

            if (confirmationsCount == 0)
            {
                await AddBroadcastedOutputs(coins, address);
            }


            coins = await FilterCoins(coins);

            return coins;
        }

        public async Task SaveOuputs(Transaction tx)
        {
            await _spentOutputService.SaveSpentOutputs(tx);
            await _broadcastedOutputsService.SaveNewOutputs(tx);
        }

        private async Task AddBroadcastedOutputs(List<CoinWithSettlementInfo> coins, string walletAddress)
        {
            var set = new HashSet<OutPoint>(coins.Select(x => x.Outpoint));

            var internalSavedOutputs = (await _broadcastedOutputsService.GetOutputs(walletAddress))
                .Where(o => !set.Contains(new OutPoint(uint256.Parse(o.TransactionHash), o.N)));

            coins.AddRange(internalSavedOutputs.Select(o =>
            {
                var coin = new CoinWithSettlementInfo(new OutPoint(uint256.Parse(o.TransactionHash), o.N),
                    new TxOut(new Money(o.Amount, MoneyUnit.Satoshi), o.ScriptPubKey.ToScript()), isSettled: false);

                return coin;
            }));
        }

        private async Task<List<CoinWithSettlementInfo>> FilterCoins(List<CoinWithSettlementInfo> coins)
        {
            var spentcoins = await _spentOutputService.GetUnspentOutputs(coins.Select(o => new Output(o.Outpoint)));

            var spentSet = new HashSet<OutPoint>(spentcoins.Select(x => new OutPoint(uint256.Parse(x.TransactionHash), x.N)));
            
            return coins.Where(o => !spentSet.Contains(o.Outpoint)).ToList();
        }

    }
}
