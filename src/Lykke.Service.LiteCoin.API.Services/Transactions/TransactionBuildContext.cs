using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.OpenAsset;

namespace Lykke.Service.LiteCoin.API.Services.Transactions
{
    public class TransactionBuildContext
    {
        public Network Network { get; }

        public TransactionBuildContext(Network network)
        {
            Network = network;
        }

        public List<ICoin> Coins { get; set; } = new List<ICoin>();

        public List<ICoin> FeeCoins { get; set; } = new List<ICoin>();


        public AssetId IssuedAssetId { get; private set; }

        public void AddCoins(IEnumerable<ICoin> coins, bool feeCoin = false)
        {
            AddCoins(feeCoin, coins.ToArray());
        }

        public void AddCoins(bool feeCoin = false, params ICoin[] coins)
        {
            Coins.AddRange(coins);
            if (feeCoin)
                FeeCoins.AddRange(coins);
        }
        


        public async Task<T> Build<T>(Func<Task<T>> buildAction)
        {
            try
            {
                return await buildAction();
            }
            catch (Exception)
            {
                //if (FeeCoins.Count > 0)
                //{
                //    var queue = _pregeneratedOutputsQueueFactory.CreateFeeQueue();
                //    await queue.EnqueueOutputs(FeeCoins.OfType<Coin>().ToArray());
                //}
                //foreach (var extraAmount in _extraAmounts)
                //{
                //    await _extraAmountRepository.Decrease(extraAmount);
                //}
                throw;
            }
        }
    }
}
